using System;
using System.Reflection.Emit;
using CompilerSimpleCSharp.TableSymbols;

namespace CompilerSimpleCSharp
{
    internal class Parser
    {
        private Scanner scanner;
        private Token ct;//Пореден токен
        private Table symbolT;//Символ от таблицата
        private Emit emit;

        public Parser(Scanner scanner, Table symbolT, Emit emit){
            this.emit = emit;
            this.scanner = scanner;
            this.symbolT = symbolT;
            ReadNextToken();
        }

        internal bool Parse(){
            while (IsStatement()) ;
            emit.ReadKey();
            emit.AddPop();
            return (ct is EOFToken);
        }

        #region IsA...
        private bool IsStatement(){
            if (IsExpression()){
                if (!CheckSpecialSymbol(";")){
                    Console.WriteLine("Очаквам специален символ  ';41'");
                    return false;
                }

                emit.AddPop();
                return true;
            }
            if (!CheckSpecialSymbol(";48"))
                return false;

            return true;
        }
        private bool ZoomStatement()
        {
            if (CheckKeyword("zoom"))
            {
                if (!CheckSpecialSymbol("("))
                {
                    Console.WriteLine("Expected specia symbol '('");
                    return false;
                }
                if (!IsExpression())
                {
                    Console.WriteLine("Очаквам Expression");
                    return false;
                }
                if (!CheckSpecialSymbol(")"))

                    if (!CheckSpecialSymbol(")"))
                    {
                        Console.WriteLine("Expected specia symbol ')'");
                        return false;
                    }
                if (!IsStatement())
                {
                    Console.WriteLine("Очаквам Statement");
                    return false;
                }
                return true;
            }
            return false;

        }

        private bool IsExpression(){
            if (IsBitwiseExpression()){
                return true;
            }
            return false;
        }

        private bool IsBitwiseExpression(){
            if (IsAdditiveExpression()){
                return true;
            }
            return false;
        }

        //AdditiveExpression = MultiplicativeExpression {('+' | '-') MultiplicativeExpression}
        private bool IsAdditiveExpression(){
            if (IsMultiplicativeExpression())
            {
                if (CheckSpecialSymbol("+"))
                {
                    if (!IsAdditiveExpression())
                    {
                        Console.WriteLine("Заявка за събиране...");
                        return false;
                    }
                    emit.AddPlus();
                }
                if (CheckSpecialSymbol("-")){
                    if (!IsAdditiveExpression()){
                        Console.WriteLine("Заявка за изваждане...");
                        return false;
                    }
                    emit.AddMinus();
                }
                return true;
            }
            return false;
        }

        //AndExpression = OrExpression {('|' | '||') MultiplicativeExpression}
        private bool IsAndOrExpression()
        {
            if (IsAndOrExpression())
            {
                if (CheckSpecialSymbol("|"))
                {
                    if (!IsAdditiveExpression())
                    {
                        Console.WriteLine("Заявка с И ...");
                        return false;
                    }
                    emit.AddAnd();
                }
                if (CheckSpecialSymbol("||")){
                    if (!IsAndOrExpression()){
                        Console.WriteLine("Заявка с ИЛИ ...");
                        return false;
                    }
                    emit.AddOr();
                }
                return true;
            }
            return false;
        }


        //MultiplicativeExpression = PrimaryExpression {('*' | '/' | '%') PrimaryExpression}
        private bool IsMultiplicativeExpression(){
            if (IsPrimaryExpression()){
                if (CheckSpecialSymbol("*")){
                    if (!IsMultiplicativeExpression()){
                        Console.WriteLine("Заявка за умножение...");
                        return false;
                    }
                    emit.AddMul();
                }
                if (CheckSpecialSymbol("/")){
                    if (!IsMultiplicativeExpression()){
                        Console.WriteLine("Заявка за делене...");
                        return false;
                    }
                    emit.AddDiv();
                }
                if (CheckSpecialSymbol("%")){
                    if (!IsMultiplicativeExpression()){
                        Console.WriteLine("Заявка за модулно делене...");
                        return false;
                    }
                    emit.AddRem();
                }

                return true;
            }
            return false;
        }

        ///    PrimaryExpression = Ident['=' Expression] | '~' PrimaryExpression |
        ///      '++' Ident | '--' Ident |Ident/ '++' | Ident '--' | 
		///     Number | PrintFunc | ScanfFunc | '(' Expression ')'
        private bool IsPrimaryExpression(){

            Token tempToken = ct;

            if (CheckIdent()){
                LocalVariableSymbol localVar = this.GetLocalVariableSymbol(tempToken);

                if (CheckSpecialSymbol("=")){
                    if (!IsExpression()){
                        Console.WriteLine("Очаквам израз...");
                        return false;
                    }
                    emit.AddLocalVarAssigment(localVar.localVariableInfo);
                    emit.AddGetLocalVar(localVar.localVariableInfo);
                    return true;
                }
                if (CheckSpecialSymbol("++")){
                    emit.AddGetLocalVar(localVar.localVariableInfo);
                    emit.AddDuplicate();
                    emit.AddGetNumber(1);
                    emit.AddPlus();
                    emit.AddLocalVarAssigment(localVar.localVariableInfo);
                    return true;
                }
                if (CheckSpecialSymbol("--")){
                    emit.AddGetLocalVar(localVar.localVariableInfo);
                    emit.AddDuplicate();
                    emit.AddGetNumber(1);
                    emit.AddMinus();
                    emit.AddLocalVarAssigment(localVar.localVariableInfo);
                    return true;
                }
                emit.AddGetLocalVar(localVar.localVariableInfo);
                return true;
            }
            if (CheckSpecialSymbol("(")){
                if (!IsExpression())
                {
                    Console.WriteLine("Очаквам иззраз 137'");
                    return false;
                }
                if (!CheckSpecialSymbol(")")){
                    Console.WriteLine("Очаквам специален символ  ')141'");
                    return false;
                }
                return true;
            }
            if (CheckNumber()){
                emit.AddGetNumber(((NumberToken)tempToken).Value);
                return true;
            }
            if (CheckKeyword("scanf")){
                if (!CheckSpecialSymbol("(")){
                    Console.WriteLine("Очаквам специален символ... '('");
                    return false;
                }
                if (!CheckSpecialSymbol(")")){
                    Console.WriteLine("Очаквам специален символ... ')'");
                    return false;
                }
                emit.EmitReadLine();
                return true;
            }
            if (CheckKeyword("printf")){
                if (!CheckSpecialSymbol("("))
                {
                    Console.WriteLine("Очаквам специален символ...'('");
                    return false;
                }
                if (!IsExpression()){
                    Console.WriteLine("Очаквам специален символ... 'Expr'");
                    return false;
                }
                else{
                    emit.EmitWriteLine();
                    emit.AddGetNumber(0);
                }
                if (!CheckSpecialSymbol(")")){
                    Console.WriteLine("Очаквам специален символ... ')'");
                    return false;
                }
                return true;
            }
            if (CheckSpecialSymbol("++")){
                tempToken = ct;
                if (!CheckIdent()){
                    Console.WriteLine("Идентифицирана заявка!");
                    return false;
                }
                LocalVariableSymbol localVariable = this.GetLocalVariableSymbol(tempToken);
                emit.AddGetLocalVar(localVariable.localVariableInfo);
                emit.AddGetNumber(1);
                emit.AddPlus();
                emit.AddDuplicate();
                emit.AddLocalVarAssigment(localVariable.localVariableInfo);
                return true;
            }
            if (CheckSpecialSymbol("--")){
                tempToken = ct;
                if (!CheckIdent()){
                    Console.WriteLine("Идентифицирана заявка!!");
                    return false;
                }
                LocalVariableSymbol localVariable = this.GetLocalVariableSymbol(tempToken);
                emit.AddGetLocalVar(localVariable.localVariableInfo);
                emit.AddGetNumber(1);
                emit.AddMinus();
                emit.AddDuplicate();
                emit.AddLocalVarAssigment(localVariable.localVariableInfo);
                return true;
            }

            return false;
        }
        #endregion

        #region Check Token
        private bool CheckSpecialSymbol(string symbol){
            bool result = (ct is SpecialSymbolToken) && ((SpecialSymbolToken)ct).value == symbol;
            if (result) ReadNextToken();
            return result;
        }
        private bool CheckIdent(){
            bool result = (ct is IdentToken);
            if (result) ReadNextToken();
            return result;
        }

        private bool CheckKeyword(string keyword){
            bool result = (ct is KeywordToken) && ((KeywordToken)ct).value == keyword;
            if (result) ReadNextToken();
            return result;
        }
        private bool CheckNumber(){
            bool result = (ct is NumberToken);
            if (result){
                ReadNextToken();
            }
            return result;
            
        }
        #endregion

        private LocalVariableSymbol GetLocalVariableSymbol(Token token){
            IdentToken tempIdent = (IdentToken)token;
            LocalVariableSymbol localVar;

            if (!symbolT.ExistCurrentScopeSymbol(tempIdent.value)){
                LocalBuilder tmpVar = emit.AddLocalVar(tempIdent.value, typeof(int));
                localVar = symbolT.AddLocalVar(tempIdent, tmpVar);
            }
            else{
                localVar = (LocalVariableSymbol)symbolT.GetSymbol(tempIdent.value);
            }
            return localVar;
        }

        private void ReadNextToken(){
            ct = scanner.Next();
        }
    }
}