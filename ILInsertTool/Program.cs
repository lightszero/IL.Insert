using Mono.Cecil;
using System;

namespace ILInsertTool
{
    public class ILAssembly
    {
        AssemblyDefinition assembly;
        bool HadInsert = false;
        public void Load(System.IO.Stream msDll, System.IO.Stream msPdb = null)
        {
            // more simple reader func.
            var readerParameters = new ReaderParameters { ReadSymbols = (msPdb != null), SymbolStream = msPdb };
            this.assembly = AssemblyDefinition.ReadAssembly(msDll, readerParameters);




        }
        public void InsertCalcCode()
        {
            // add ref
            var assem2 = AssemblyDefinition.ReadAssembly("calctool.dll");
            foreach (var t in assem2.MainModule.Types)
            {

            }
            var type2 = assem2.MainModule.GetType("calctool.CalcTool");
            //this.assembly.Modules.Add(assem2.MainModule);
            //this.assembly.MainModule.ModuleReferences.Add(assem2.MainModule);
            var type3 = this.assembly.MainModule.ImportReference(type2);
            MethodReference method3 = null;
            foreach (var me in type3.Resolve().Methods)
            {
                if (me.Name == "Add")
                {
                    method3 = this.assembly.MainModule.ImportReference(me);
                }
            }
            foreach (Mono.Cecil.TypeDefinition type in this.assembly.MainModule.Types)
            {
                Console.WriteLine("that type:" + type.FullName);
                if (type.HasMethods)
                {
                    foreach (MethodDefinition t in type.Methods)
                    {
                        if (t.IsConstructor) continue;
                        var pdef = new ParameterDefinition("price", ParameterAttributes.None, type3);
                        var pindex = t.Parameters.Count;
                        t.Parameters.Add(pdef);
                        Console.WriteLine("that func:" + t.FullName);
                        var ilProcessor = t.Body.GetILProcessor();

                        int freecode = 0;
                        for (var i = 0; i < t.Body.Instructions.Count; i++)
                        {
                            var ins = t.Body.Instructions[i];
                            //跳转类代码之前插入计数代码
                            if (ins.OpCode.Code == Mono.Cecil.Cil.Code.Br
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Brfalse
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Brtrue
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Brfalse_S
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Brtrue_S
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Br_S
                                || ins.OpCode.Code == Mono.Cecil.Cil.Code.Ret
                             )
                            {
                                var insert1 = ilProcessor.Create(Mono.Cecil.Cil.OpCodes.Ldarg, pindex);
                                var insert2 = ilProcessor.Create(Mono.Cecil.Cil.OpCodes.Ldc_I4, freecode);
                                var insert3 = ilProcessor.Create(Mono.Cecil.Cil.OpCodes.Call, method3);
                                freecode = 0;

                                ilProcessor.InsertBefore(ins, insert1);
                                ilProcessor.InsertBefore(ins, insert2);
                                ilProcessor.InsertBefore(ins, insert3);

                                i += 3;
                            }
                            freecode++;
                        }
                    }
                }
            }
        }
        public void Save(System.IO.Stream msDll, System.IO.Stream msPdb = null)
        {
            WriterParameters wp = new WriterParameters()
            {
                WriteSymbols = (msPdb != null),
                SymbolStream = msPdb,
            };
            this.assembly.Write(msDll, wp);
        }
    }
}
