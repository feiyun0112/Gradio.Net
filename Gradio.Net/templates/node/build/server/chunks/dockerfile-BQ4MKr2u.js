function simpleMode(states) {
  ensureState(states, "start");
  var states_ = {}, meta = states.languageData || {}, hasIndentation = false;
  for (var state in states)
    if (state != meta && states.hasOwnProperty(state)) {
      var list = states_[state] = [], orig = states[state];
      for (var i = 0; i < orig.length; i++) {
        var data = orig[i];
        list.push(new Rule(data, states));
        if (data.indent || data.dedent)
          hasIndentation = true;
      }
    }
  return {
    name: meta.name,
    startState: function() {
      return { state: "start", pending: null, indent: hasIndentation ? [] : null };
    },
    copyState: function(state2) {
      var s = { state: state2.state, pending: state2.pending, indent: state2.indent && state2.indent.slice(0) };
      if (state2.stack)
        s.stack = state2.stack.slice(0);
      return s;
    },
    token: tokenFunction(states_),
    indent: indentFunction(states_, meta),
    languageData: meta
  };
}
function ensureState(states, name) {
  if (!states.hasOwnProperty(name))
    throw new Error("Undefined state " + name + " in simple mode");
}
function toRegex(val, caret) {
  if (!val)
    return /(?:)/;
  var flags = "";
  if (val instanceof RegExp) {
    if (val.ignoreCase)
      flags = "i";
    val = val.source;
  } else {
    val = String(val);
  }
  return new RegExp("^(?:" + val + ")", flags);
}
function asToken(val) {
  if (!val)
    return null;
  if (val.apply)
    return val;
  if (typeof val == "string")
    return val.replace(/\./g, " ");
  var result = [];
  for (var i = 0; i < val.length; i++)
    result.push(val[i] && val[i].replace(/\./g, " "));
  return result;
}
function Rule(data, states) {
  if (data.next || data.push)
    ensureState(states, data.next || data.push);
  this.regex = toRegex(data.regex);
  this.token = asToken(data.token);
  this.data = data;
}
function tokenFunction(states) {
  return function(stream, state) {
    if (state.pending) {
      var pend = state.pending.shift();
      if (state.pending.length == 0)
        state.pending = null;
      stream.pos += pend.text.length;
      return pend.token;
    }
    var curState = states[state.state];
    for (var i = 0; i < curState.length; i++) {
      var rule = curState[i];
      var matches = (!rule.data.sol || stream.sol()) && stream.match(rule.regex);
      if (matches) {
        if (rule.data.next) {
          state.state = rule.data.next;
        } else if (rule.data.push) {
          (state.stack || (state.stack = [])).push(state.state);
          state.state = rule.data.push;
        } else if (rule.data.pop && state.stack && state.stack.length) {
          state.state = state.stack.pop();
        }
        if (rule.data.indent)
          state.indent.push(stream.indentation() + stream.indentUnit);
        if (rule.data.dedent)
          state.indent.pop();
        var token = rule.token;
        if (token && token.apply)
          token = token(matches);
        if (matches.length > 2 && rule.token && typeof rule.token != "string") {
          state.pending = [];
          for (var j = 2; j < matches.length; j++)
            if (matches[j])
              state.pending.push({ text: matches[j], token: rule.token[j - 1] });
          stream.backUp(matches[0].length - (matches[1] ? matches[1].length : 0));
          return token[0];
        } else if (token && token.join) {
          return token[0];
        } else {
          return token;
        }
      }
    }
    stream.next();
    return null;
  };
}
function indentFunction(states, meta) {
  return function(state, textAfter) {
    if (state.indent == null || meta.dontIndentStates && meta.doneIndentState.indexOf(state.state) > -1)
      return null;
    var pos = state.indent.length - 1, rules = states[state.state];
    scan:
      for (; ; ) {
        for (var i = 0; i < rules.length; i++) {
          var rule = rules[i];
          if (rule.data.dedent && rule.data.dedentIfLineStart !== false) {
            var m = rule.regex.exec(textAfter);
            if (m && m[0]) {
              pos--;
              if (rule.next || rule.push)
                rules = states[rule.next || rule.push];
              textAfter = textAfter.slice(m[0].length);
              continue scan;
            }
          }
        }
        break;
      }
    return pos < 0 ? 0 : state.indent[pos];
  };
}
var from = "from";
var fromRegex = new RegExp("^(\\s*)\\b(" + from + ")\\b", "i");
var shells = ["run", "cmd", "entrypoint", "shell"];
var shellsAsArrayRegex = new RegExp("^(\\s*)(" + shells.join("|") + ")(\\s+\\[)", "i");
var expose = "expose";
var exposeRegex = new RegExp("^(\\s*)(" + expose + ")(\\s+)", "i");
var others = [
  "arg",
  "from",
  "maintainer",
  "label",
  "env",
  "add",
  "copy",
  "volume",
  "user",
  "workdir",
  "onbuild",
  "stopsignal",
  "healthcheck",
  "shell"
];
var instructions = [from, expose].concat(shells).concat(others), instructionRegex = "(" + instructions.join("|") + ")", instructionOnlyLine = new RegExp("^(\\s*)" + instructionRegex + "(\\s*)(#.*)?$", "i"), instructionWithArguments = new RegExp("^(\\s*)" + instructionRegex + "(\\s+)", "i");
const dockerFile = simpleMode({
  start: [
    // Block comment: This is a line starting with a comment
    {
      regex: /^\s*#.*$/,
      sol: true,
      token: "comment"
    },
    {
      regex: fromRegex,
      token: [null, "keyword"],
      sol: true,
      next: "from"
    },
    // Highlight an instruction without any arguments (for convenience)
    {
      regex: instructionOnlyLine,
      token: [null, "keyword", null, "error"],
      sol: true
    },
    {
      regex: shellsAsArrayRegex,
      token: [null, "keyword", null],
      sol: true,
      next: "array"
    },
    {
      regex: exposeRegex,
      token: [null, "keyword", null],
      sol: true,
      next: "expose"
    },
    // Highlight an instruction followed by arguments
    {
      regex: instructionWithArguments,
      token: [null, "keyword", null],
      sol: true,
      next: "arguments"
    },
    {
      regex: /./,
      token: null
    }
  ],
  from: [
    {
      regex: /\s*$/,
      token: null,
      next: "start"
    },
    {
      // Line comment without instruction arguments is an error
      regex: /(\s*)(#.*)$/,
      token: [null, "error"],
      next: "start"
    },
    {
      regex: /(\s*\S+\s+)(as)/i,
      token: [null, "keyword"],
      next: "start"
    },
    // Fail safe return to start
    {
      token: null,
      next: "start"
    }
  ],
  single: [
    {
      regex: /(?:[^\\']|\\.)/,
      token: "string"
    },
    {
      regex: /'/,
      token: "string",
      pop: true
    }
  ],
  double: [
    {
      regex: /(?:[^\\"]|\\.)/,
      token: "string"
    },
    {
      regex: /"/,
      token: "string",
      pop: true
    }
  ],
  array: [
    {
      regex: /\]/,
      token: null,
      next: "start"
    },
    {
      regex: /"(?:[^\\"]|\\.)*"?/,
      token: "string"
    }
  ],
  expose: [
    {
      regex: /\d+$/,
      token: "number",
      next: "start"
    },
    {
      regex: /[^\d]+$/,
      token: null,
      next: "start"
    },
    {
      regex: /\d+/,
      token: "number"
    },
    {
      regex: /[^\d]+/,
      token: null
    },
    // Fail safe return to start
    {
      token: null,
      next: "start"
    }
  ],
  arguments: [
    {
      regex: /^\s*#.*$/,
      sol: true,
      token: "comment"
    },
    {
      regex: /"(?:[^\\"]|\\.)*"?$/,
      token: "string",
      next: "start"
    },
    {
      regex: /"/,
      token: "string",
      push: "double"
    },
    {
      regex: /'(?:[^\\']|\\.)*'?$/,
      token: "string",
      next: "start"
    },
    {
      regex: /'/,
      token: "string",
      push: "single"
    },
    {
      regex: /[^#"']+[\\`]$/,
      token: null
    },
    {
      regex: /[^#"']+$/,
      token: null,
      next: "start"
    },
    {
      regex: /[^#"']+/,
      token: null
    },
    // Fail safe return to start
    {
      token: null,
      next: "start"
    }
  ],
  languageData: {
    commentTokens: { line: "#" }
  }
});

export { dockerFile };
//# sourceMappingURL=dockerfile-BQ4MKr2u.js.map