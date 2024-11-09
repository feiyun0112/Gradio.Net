import { L as LRParser } from './index12-BtJKZaSL.js';
import { s as styleTags, t as tags, L as LRLanguage, i as indentNodeProp, w as continuedIndent, f as foldNodeProp, c as foldInside, e as LanguageSupport } from './Index13-30CwZXhj.js';
import './ssr-RaXq3SJh.js';
import './2-B6LMYTAg.js';
import './index-hSrgoQUm.js';
import 'tty';
import 'path';
import 'url';
import 'fs';
import './Component-Dv7eSVA_.js';
import './DownloadLink--4obEanq.js';
import './Example6-IJeAY36h.js';

const jsonHighlighting = styleTags({
  String: tags.string,
  Number: tags.number,
  "True False": tags.bool,
  PropertyName: tags.propertyName,
  Null: tags.null,
  ",": tags.separator,
  "[ ]": tags.squareBracket,
  "{ }": tags.brace
});
const parser = LRParser.deserialize({
  version: 14,
  states: "$bOVQPOOOOQO'#Cb'#CbOnQPO'#CeOvQPO'#CjOOQO'#Cp'#CpQOQPOOOOQO'#Cg'#CgO}QPO'#CfO!SQPO'#CrOOQO,59P,59PO![QPO,59PO!aQPO'#CuOOQO,59U,59UO!iQPO,59UOVQPO,59QOqQPO'#CkO!nQPO,59^OOQO1G.k1G.kOVQPO'#ClO!vQPO,59aOOQO1G.p1G.pOOQO1G.l1G.lOOQO,59V,59VOOQO-E6i-E6iOOQO,59W,59WOOQO-E6j-E6j",
  stateData: "#O~OcOS~OQSORSOSSOTSOWQO]ROePO~OVXOeUO~O[[O~PVOg^O~Oh_OVfX~OVaO~OhbO[iX~O[dO~Oh_OVfa~OhbO[ia~O",
  goto: "!kjPPPPPPkPPkqwPPk{!RPPP!XP!ePP!hXSOR^bQWQRf_TVQ_Q`WRg`QcZRicQTOQZRQe^RhbRYQR]R",
  nodeNames: "⚠ JsonText True False Null Number String } { Object Property PropertyName ] [ Array",
  maxTerm: 25,
  nodeProps: [
    ["openedBy", 7, "{", 12, "["],
    ["closedBy", 8, "}", 13, "]"]
  ],
  propSources: [jsonHighlighting],
  skippedNodes: [0],
  repeatNodeCount: 2,
  tokenData: "(|~RaXY!WYZ!W]^!Wpq!Wrs!]|}$u}!O$z!Q!R%T!R![&c![!]&t!}#O&y#P#Q'O#Y#Z'T#b#c'r#h#i(Z#o#p(r#q#r(w~!]Oc~~!`Wpq!]qr!]rs!xs#O!]#O#P!}#P;'S!];'S;=`$o<%lO!]~!}Oe~~#QXrs!]!P!Q!]#O#P!]#U#V!]#Y#Z!]#b#c!]#f#g!]#h#i!]#i#j#m~#pR!Q![#y!c!i#y#T#Z#y~#|R!Q![$V!c!i$V#T#Z$V~$YR!Q![$c!c!i$c#T#Z$c~$fR!Q![!]!c!i!]#T#Z!]~$rP;=`<%l!]~$zOh~~$}Q!Q!R%T!R![&c~%YRT~!O!P%c!g!h%w#X#Y%w~%fP!Q![%i~%nRT~!Q![%i!g!h%w#X#Y%w~%zR{|&T}!O&T!Q![&Z~&WP!Q![&Z~&`PT~!Q![&Z~&hST~!O!P%c!Q![&c!g!h%w#X#Y%w~&yOg~~'OO]~~'TO[~~'WP#T#U'Z~'^P#`#a'a~'dP#g#h'g~'jP#X#Y'm~'rOR~~'uP#i#j'x~'{P#`#a(O~(RP#`#a(U~(ZOS~~(^P#f#g(a~(dP#i#j(g~(jP#X#Y(m~(rOQ~~(wOW~~(|OV~",
  tokenizers: [0],
  topRules: { "JsonText": [0, 1] },
  tokenPrec: 0
});
const jsonParseLinter = () => (view) => {
  try {
    JSON.parse(view.state.doc.toString());
  } catch (e) {
    if (!(e instanceof SyntaxError))
      throw e;
    const pos = getErrorPosition(e, view.state.doc);
    return [{
      from: pos,
      message: e.message,
      severity: "error",
      to: pos
    }];
  }
  return [];
};
function getErrorPosition(error, doc) {
  let m;
  if (m = error.message.match(/at position (\d+)/))
    return Math.min(+m[1], doc.length);
  if (m = error.message.match(/at line (\d+) column (\d+)/))
    return Math.min(doc.line(+m[1]).from + +m[2] - 1, doc.length);
  return 0;
}
const jsonLanguage = /* @__PURE__ */ LRLanguage.define({
  name: "json",
  parser: /* @__PURE__ */ parser.configure({
    props: [
      /* @__PURE__ */ indentNodeProp.add({
        Object: /* @__PURE__ */ continuedIndent({ except: /^\s*\}/ }),
        Array: /* @__PURE__ */ continuedIndent({ except: /^\s*\]/ })
      }),
      /* @__PURE__ */ foldNodeProp.add({
        "Object Array": foldInside
      })
    ]
  }),
  languageData: {
    closeBrackets: { brackets: ["[", "{", '"'] },
    indentOnInput: /^\s*[\}\]]$/
  }
});
function json() {
  return new LanguageSupport(jsonLanguage);
}

export { json, jsonLanguage, jsonParseLinter };
//# sourceMappingURL=index32-Cep11LE9.js.map
