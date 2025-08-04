#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ateo.StateManagement;
using UnityEngine;

namespace Ateo.CodeGeneration
{
    // classes dont need to be static when you are using InitializeOnLoad
    [InitializeOnLoad]
    public class CodeGeneratorStates
    {
        private static CodeGeneratorStates _instance;
        private CodeGeneratorCommon _common = new CodeGeneratorCommon();
        private static CodeGeneratorCommon Com => _instance._common;

        public const string FolderName = "Application/States";
        public const string FileName = "States";
        
        private const string NameSpace = "Ateo.StateManagement";

        public static string FolderPath => string.Format(CodeGeneratorCommon.PathFormat, CodeGeneratorCommon.DirPath, FolderName);
        public static string FileNamePlusExtension => string.Format(CodeGeneratorCommon.FileFormat, FileName);

        // static constructor
        static CodeGeneratorStates()
        {
            _instance = new CodeGeneratorStates
            {
                _common = new CodeGeneratorCommon()
            };
        }

        private static IEnumerable<string> GetNewNames()
        {
#if STATEMACHINE
            return StateHelper.GetStateNames();
#else
            return new List<string>();
#endif
        }

        // writes a file to the project folder
        [MenuItem("Tools/State Management/Update States")]
        public static void GenerateStates()
        {
            WriteCodeFile();
        }

        public static void GenerateStates(List<AddState> addStates, List<string> removeStates)
        {
            WriteCodeFile(addStates, removeStates);
        }

        private static void WriteCodeFile(List<AddState> addStates = null, List<string> removeStates = null)
        {
            if (addStates == null)
                addStates = new List<AddState>();

            if (removeStates == null)
                removeStates = new List<string>();
            
            List<string> states = GetNewNames().ToList();
            
            for (int i = addStates.Count - 1; i >= 0; i--)
            {
                AddState state = addStates[i];
                
                if (string.IsNullOrEmpty(state.Name) || string.IsNullOrWhiteSpace(state.Name) || !Regex.IsMatch(state.Name, "^[a-zA-Z0-9 ]*$"))
                {
                    Debug.LogError($"CodeGenerator: New State '{state}' contains invalid characters. --> removed");
                    addStates.RemoveAt(i);
                }
                else if(char.IsLower(state.Name[0]))
                {
                    Debug.LogWarning($"CodeGenerator: New State '{state}' starts with a lower case character. --> fixed automatically");
                    
                    if (state.Name.Length == 1)
                    {
                        state.Name = addStates[i].Name = char.ToUpper(state.Name[0]).ToString();
                    }
                    else
                    {
                        state.Name = addStates[i].Name = char.ToUpper(state.Name[0]) + state.Name.Substring(1);
                    }
                }
                
                if (states.Contains(state.Name))
                {
                    Debug.LogWarning($"CodeGenerator: New State '{state}' already exists. --> removed");
                    addStates.RemoveAt(i);
                }
            }
            
            for (int i = removeStates.Count - 1; i >= 0; i--)
            {
                string state = removeStates[i];
                
                if (string.IsNullOrEmpty(state) || string.IsNullOrWhiteSpace(state))
                {
                    removeStates.RemoveAt(i);
                }
            }

            foreach (AddState state in addStates)
            {
                string fileName = $"{state.Name}.cs";

                if (!File.Exists(Path.Combine(FolderPath, fileName)))
                {
                    CodeGeneratorCommon.WriteCodeFile(FolderPath, fileName,
                        builder => { builder.AppendFormat(CodeGeneratorCommon.StateFormat, state.Name, state.Parent != null ? $"States.{state.Parent.Name}" : "null"); });
                }
            }

            CodeGeneratorCommon.WriteCodeFile(FolderPath, FileNamePlusExtension, builder =>
            {
                WrappedInt indentCount = 0;
                builder.AppendIndentLine(indentCount, CodeGeneratorCommon.AutoGenTemplate);
                builder.AppendIndentLine(indentCount, string.Format(CodeGeneratorCommon.NameSpaceFormat, NameSpace));

                foreach (AddState state in addStates)
                {
                    if (!states.Contains(state.Name))
                    {
                        states.Add(state.Name);
                    }
                }

                states.Sort();

                using (new CurlyIndent(builder, indentCount))
                {
                    builder.AppendIndentFormatLine(indentCount, "public static class {0}", FileName);
                    using (new CurlyIndent(builder, indentCount))
                    {
                        foreach (string name in states)
                        {
                            if (!removeStates.Contains(name))
                            {
                                builder.AppendIndentFormatLine(indentCount,
                                    $"public static readonly IState {name} = StateManagement.{name}.Instance;");
                            }
                        }
                    }

                    builder.Append(Environment.NewLine);
                    builder.AppendIndentFormatLine(indentCount, "public enum {0}Enum", FileName);
                    using (new CurlyIndent(builder, indentCount))
                    {
                        builder.AppendIndentFormatLine(indentCount, "None,");
                        foreach (string name in states)
                        {
                            if (!removeStates.Contains(name))
                            {
                                builder.AppendIndentFormatLine(indentCount, $"{name},");
                            }
                        }
                    }
                }

                foreach (string state in removeStates)
                {
                    string file = Path.Combine(FolderPath, $"{state}.cs");

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    string meta = Path.Combine(FolderPath, $"{state}.cs.meta");

                    if (File.Exists(Path.Combine(meta)))
                    {
                        File.Delete(meta);
                    }
                }
            });
        }
    }
}
#endif