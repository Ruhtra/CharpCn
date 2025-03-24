using Shadcn.Components.Priv.Clsx;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Shadcn.Components.Priv.Cva {

    /// <summary>
    /// Simula a função "clsx" – concatena os valores não nulos em uma única string separada por espaços.
    /// </summary>

    #region Hooks and DefineConfig Options

    public class Hooks {
        /// <summary>
        /// @deprecated use OnComplete
        /// </summary>
        public Func<string, string> CxDone { get; set; }

        /// <summary>
        /// Retorna a string completa dos nomes concatenados.
        /// </summary>
        public Func<string, string> OnComplete { get; set; }
    }

    public class DefineConfigOptions {
        public Hooks Hooks { get; set; }
    }

    #endregion

    #region Delegates and Result Types

    // Delegate que representa a função CX (concatenação de classes)
    public delegate string CXDelegate(params object[] inputs);

    // Delegate que representa a função CVA: recebe um objeto "props" e retorna uma string.
    public delegate string CVADelegate(Dictionary<string, object> props = null);

    // Delegate que representa a fábrica de funções CVA: recebe um config e retorna uma função CVADelegate.
    public delegate CVADelegate CVAFactoryDelegate(CVAConfig config);

    // Delegate que representa a função de composição: recebe "props" e retorna uma string.
    public delegate string ComposeDelegate(Dictionary<string, object> props = null);

    // Delegate que representa a fábrica de funções compose: recebe um array de funções e retorna uma função ComposeDelegate.
    public delegate ComposeDelegate ComposeFactoryDelegate(params Func<Dictionary<string, object>, string>[] components);

    public class DefineConfigResult {
        public ComposeFactoryDelegate Compose { get; set; }
        public CVAFactoryDelegate CVA { get; set; }
        public CXDelegate CX { get; set; }
    }

    #endregion

    #region CVA Config and Compound Variant

    public class CVAConfig {
        /// <summary>
        /// Valor base para as classes.
        /// </summary>
        public object Base { get; set; }

        /// <summary>
        /// Dicionário de variantes. Cada chave representa o nome da variante e seu valor é um dicionário
        /// que mapeia a string (ou valor convertido) para a classe correspondente.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> Variants { get; set; }

        /// <summary>
        /// Lista de variantes compostas.
        /// </summary>
        public List<CompoundVariant> CompoundVariants { get; set; }

        /// <summary>
        /// Variantes padrão.
        /// </summary>
        public Dictionary<string, object> DefaultVariants { get; set; }
    }

    /// <summary>
    /// Representa uma variante composta, contendo os critérios e as classes associadas.
    /// </summary>
    public class CompoundVariant {
        /// <summary>
        /// Critérios que devem ser verificados. As chaves são os nomes das variantes e os valores os valores esperados.
        /// </summary>
        public Dictionary<string, object> Criteria { get; set; }

        /// <summary>
        /// Classe opcional.
        /// </summary>
        public object Class { get; set; }

        /// <summary>
        /// ClassName opcional.
        /// </summary>
        public object ClassName { get; set; }
    }

    #endregion

    public static class DefineConfig {
        public static DefineConfigResult Create(DefineConfigOptions options = null) {
            // Implementação do "cx"
            CXDelegate cx = (inputs) => {
                string result = ClsxHelper.Clsx(inputs);
                if (options?.Hooks?.CxDone != null) {
                    return options.Hooks.CxDone(result);
                }
                if (options?.Hooks?.OnComplete != null) {
                    return options.Hooks.OnComplete(result);
                }
                return result;
            };

            // Implementação do "cva"
            CVAFactoryDelegate cva = (CVAConfig config) => {
                return (Dictionary<string, object> props) => {
                    props = props ?? new Dictionary<string, object>();

                    // Se não houver variantes, apenas concatena a classe base com "class" e "className" de props.
                    if (config.Variants == null) {
                        return cx(config.Base,
                                  props.ContainsKey("class") ? props["class"] : null,
                                  props.ContainsKey("className") ? props["className"] : null);
                    }

                    var variantClassNames = new List<object>();

                    // Para cada variante definida, obtém o valor da propriedade e a variante padrão (se houver).
                    foreach (var variant in config.Variants) {
                        string variantName = variant.Key;
                        object propValue = props.ContainsKey(variantName) ? props[variantName] : null;
                        object defaultValue = (config.DefaultVariants != null && config.DefaultVariants.ContainsKey(variantName))
                            ? config.DefaultVariants[variantName]
                            : null;

                        // Usa o valor da prop se existir, senão usa o default.
                        string keyToUse = FalsyToString(propValue) ?? FalsyToString(defaultValue);
                        if (keyToUse != null && variant.Value.ContainsKey(keyToUse)) {
                            variantClassNames.Add(variant.Value[keyToUse]);
                        }
                    }

                    // Combina as propriedades padrão com as props (excluindo valores nulos)
                    var defaultsAndProps = new Dictionary<string, object>();
                    if (config.DefaultVariants != null) {
                        foreach (var kv in config.DefaultVariants) {
                            defaultsAndProps[kv.Key] = kv.Value;
                        }
                    }
                    foreach (var kv in props) {
                        if (kv.Value != null) {
                            defaultsAndProps[kv.Key] = kv.Value;
                        }
                    }

                    var compoundVariantClassNames = new List<object>();

                    if (config.CompoundVariants != null) {
                        foreach (var cv in config.CompoundVariants) {
                            bool matches = cv.Criteria.All(criteria => {
                                object selectorValue = defaultsAndProps.ContainsKey(criteria.Key) ? defaultsAndProps[criteria.Key] : null;

                                // Se o critério for uma lista, verifica se contém o valor; senão, compara diretamente.
                                if (criteria.Value is IEnumerable<object> list) {
                                    return list.Contains(selectorValue);
                                } else {
                                    return Equals(selectorValue, criteria.Value);
                                }
                            });

                            if (matches) {
                                compoundVariantClassNames.Add(cv.Class);
                                compoundVariantClassNames.Add(cv.ClassName);
                            }
                        }
                    }

                    // Concatena os argumentos: base, variantes, variantes compostas e classes adicionais de props.
                    var allArgs = new List<object>();
                    allArgs.Add(config.Base);
                    allArgs.AddRange(variantClassNames);
                    allArgs.AddRange(compoundVariantClassNames);
                    if (props.ContainsKey("class"))
                        allArgs.Add(props["class"]);
                    if (props.ContainsKey("className"))
                        allArgs.Add(props["className"]);

                    return cx(allArgs.ToArray());
                };
            };

            // Implementação do "compose"
            ComposeFactoryDelegate compose = (params Func<Dictionary<string, object>, string>[] components) => {
                return (Dictionary<string, object> props) => {
                    props = props ?? new Dictionary<string, object>();

                    // Cria uma cópia das props removendo as chaves "class" e "className"
                    var propsWithoutClass = props
                        .Where(kvp => kvp.Key != "class" && kvp.Key != "className")
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    // Mapeia cada componente passando as props sem "class" e "className"
                    var results = components.Select(component => component(propsWithoutClass)).ToList();

                    var allArgs = new List<object>();
                    allArgs.AddRange(results);
                    if (props.ContainsKey("class"))
                        allArgs.Add(props["class"]);
                    if (props.ContainsKey("className"))
                        allArgs.Add(props["className"]);

                    return cx(allArgs.ToArray());
                };
            };

            return new DefineConfigResult {
                Compose = compose,
                CVA = cva,
                CX = cx
            };
        }

        /// <summary>
        /// Função auxiliar que replica a lógica de "falsyToString" do TypeScript.
        /// Se o valor for boolean, retorna "true" ou "false" em letras minúsculas.
        /// Se o valor for numérico igual a zero, retorna "0".
        /// Caso contrário, retorna o ToString() do valor.
        /// </summary>
        /// <param name="value">Valor de entrada</param>
        /// <returns>Representação em string ou null</returns>
        private static string FalsyToString(object value) {
            if (value == null)
                return null;
            if (value is bool b)
                return b.ToString().ToLower();
            if (value is int i && i == 0)
                return "0";
            return value.ToString();
        }

    }

}
