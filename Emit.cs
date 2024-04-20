using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace CompilerSimpleCSharp
{
    internal class Emit{
        private AssemblyBuilder assembly;
        private ModuleBuilder module;
        private Table symbolTable;
        private TypeBuilder type;
        private ConstructorBuilder cctor;
        private ILGenerator il;
        private MethodBuilder method;
        private string executableName;

        public Emit(string name, Table symbolTable){
            this.symbolTable = symbolTable;
            this.executableName = name;
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = Path.GetFileNameWithoutExtension(name);

            string moduleName = Path.GetFileName(name);

            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
            module = assembly.DefineDynamicModule(assemblyName + "Module", moduleName);
            
        }

        internal void InitProgram(){
            //Генератор за  клас
            type = module.DefineType("program");

            //Генератор за конструктор
            cctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] {});
            ILGenerator iLGenerator = cctor.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ret);


            //Генератор за main метод
            method = type.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static);
            method.InitLocals = true;
            il = method.GetILGenerator();
        }

        internal Type WriteExecutable(){
            
            il.Emit(OpCodes.Ret);

            Type retValue = type.CreateType();
            assembly.SetEntryPoint(method);
            assembly.Save(executableName);
            return retValue;
        }

        internal void AddGetLocalVar(LocalVariableInfo localVariableInfo){
            il.Emit(OpCodes.Ldloc, (LocalBuilder)localVariableInfo);
        }

        internal void AddLocalVarAssigment(LocalVariableInfo localVariableInfo){
            il.Emit(OpCodes.Stloc, (LocalBuilder)localVariableInfo);
        }

        internal LocalBuilder AddLocalVar(string value, Type type)
{
            LocalBuilder result = il.DeclareLocal(type);
            if (!type.IsValueType){
                il.Emit(OpCodes.Newobj, type);
                il.Emit(OpCodes.Stloc, result);
            }

            return result;
        }

        internal void AddPop(){
            il.Emit(OpCodes.Pop);
        }

        internal void AddGetNumber(long value){
            if (value >= Int32.MinValue && value <= Int32.MaxValue){
                il.Emit(OpCodes.Ldc_I4, (Int32)value);
            }
            else{
                il.Emit(OpCodes.Ldc_I8, value);
            }
        }        

        internal void AddPlus()
        {
           il.Emit(OpCodes.Add);
        }        

        internal void AddMinus()
        {
           il.Emit(OpCodes.Sub);
        }

        internal void AddMul()
        {
           il.Emit(OpCodes.Mul);
        }

        internal void AddDiv()
        {
           il.Emit(OpCodes.Div);
        }
        internal void AddAnd()
        {
            il.Emit(OpCodes.And);
        }
        internal void AddOr()
        {
            il.Emit(OpCodes.Or);
        }
        internal void AddRem()
        {
           il.Emit(OpCodes.Rem);
        }

        internal void EmitReadLine()
        {
            MethodInfo readLineMetodInfo = typeof(Console).GetMethod("ReadLine", new Type[0]);
            MethodInfo convertInt32MetodInfo = typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(string) });

           il.EmitCall(OpCodes.Call, readLineMetodInfo, null);
           il.EmitCall(OpCodes.Call, convertInt32MetodInfo, null);
        }

        internal void EmitWriteLine()
        {

            MethodInfo writeMetodInfo = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) });
           il.EmitCall(OpCodes.Call, writeMetodInfo, null);
        }

        internal void ReadKey()
        {
           il.Emit(OpCodes.Call, (typeof(Console)).GetMethod("ReadKey", new Type[0]));
        }

        internal void AddDuplicate()
        {
           il.Emit(OpCodes.Dup);
        }
    }
}
