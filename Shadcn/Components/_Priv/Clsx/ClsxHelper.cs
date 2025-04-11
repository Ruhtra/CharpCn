namespace Shadcn.Components.Priv.Clsx;

using System;
using System.Collections;
using System.Text;

public static class ClsxHelper {
    private static string ToVal(object mix) {
        if (mix == null) return string.Empty;
        var str = new StringBuilder();

        switch (mix) {
            case string s:
                str.Append(s);
                break;

            case IEnumerable enumerable when !(enumerable is IDictionary):  // Trata arrays ou coleções
                foreach (var item in enumerable) {
                    var value = ToVal(item);
                    if (!string.IsNullOrEmpty(value)) {
                        if (str.Length > 0) str.Append(' ');
                        str.Append(value);
                    }
                }
                break;

            case IDictionary dictionary:  // Trata dicionários
                foreach (DictionaryEntry entry in dictionary) {
                    if (entry.Value is bool isTruthy && isTruthy) {
                        if (str.Length > 0) str.Append(' ');
                        str.Append(entry.Key.ToString());
                    }
                }
                break;

            default:
                // Se for um número ou outro tipo primitivo, converta para string
                if (mix is int || mix is float || mix is double || mix is bool) {
                    str.Append(mix.ToString());
                } else {
                    throw new ArgumentException("Tipo não suportado");
                }
                break;
        }

        return str.ToString();
    }

    public static string Clsx(params object[] args) {
        var str = new StringBuilder();
        foreach (var arg in args) {
            var tmp = ToVal(arg);
            if (!string.IsNullOrEmpty(tmp)) {
                if (str.Length > 0) str.Append(' ');
                str.Append(tmp);
            }
        }
        return str.ToString();
    }
}
