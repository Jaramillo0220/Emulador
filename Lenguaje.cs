/*
    -------------------------REQUERIMIENTOS TERCER PARCIAL----------------------------
    1) Exception en console.read() [DONE]
    2) La segunda asignacion del for(incremento) debe de 
    ejecutarse despues del bloque de instrucciones e instruccion []
    3) Programar el metodo funcionMatematica() [DONE]
    4) Programar el for []
    5) Programar el while [DONE]
    ---------------------------------------------------------------------------------
    ***********************************************************************************
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Lenguaje : Sintaxis
    {
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maxTipo;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maxTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maxTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayLista()
        {
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.getTipoDato()} {elemento.getValor()}");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            Main();
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?

        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?

        private void Variables()
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }
            //l.Add(new Variable(t, getContenido()));
            Variable v = new Variable(t, Contenido);
            l.Add(v);

            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        int r = Console.Read();
                        v.setValor(r);
                        // Asignamos el último valor leído a la última variable detectada
                    }
                    else
                    {
                        match("ReadLine");
                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float valor))
                        {
                            v.setValor(valor);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    // Como no se ingresó un número desde el Console, entonces viene de una expresión matemática
                    Expresion();
                    float resultado = s.Pop();
                    l.Last().setValor(resultado);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            if (Contenido == "Console")
            {
                console(ejecuta);
            }
            else if (Contenido == "if")
            {
                If(ejecuta);
            }
            else if (Contenido == "while")
            {
                While(ejecuta);
            }
            else if (Contenido == "do")
            {
                Do(ejecuta);
            }
            else if (Contenido == "for")
            {
                For(ejecuta);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        //Asignacion -> Identificador = Expresion; (DONE)
        /*
        Id++ (DONE)
        Id-- (DONE)
        Id IncrementoTermino Expresion (DONE)
        Id IncrementoFactor Expresion (DONE)
        Id = Console.Read() (DONE)
        Id = Console.ReadLine() (DONE)
        */

        //4) Implementar maxTipo en la asignacion, es decir, cuando se haga v.setValor(r)
        private void Asignacion()
        {
            maxTipo = Variable.TipoDato.Char;
            float r;
            // Mensaje de depuración para ver qué contiene "Contenido"
            //Console.WriteLine($"Buscando variable: {Contenido} en la lista de variables");

            // Imprimir todas las variables definidas en l
            //foreach (var variable in l)
            //{
            //  Console.WriteLine($"Variable definida: {variable.getNombre()}");
            //}

            // Buscar la variable en la lista
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);

            // Si no encuentra la variable, lanza el error
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
            }

            match(Tipos.Identificador);

            // Depuración adicional para ver qué token se está procesando
            //Console.WriteLine($"Contenido después de match(Tipos.Identificador): {Contenido}");
            if (Contenido == "++")
            {
                match("++");
                r = v.getValor() + 1;
                v.setValor(r);
            }
            else if (Contenido == "--")
            {
                match("--");
                r = v.getValor() - 1;
                v.setValor(r);
            }
            else if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        match("(");
                        r = Console.Read();
                        Console.ReadLine();
                        if (r >= 48 && r <= 57)
                        {
                            v?.setValor(r);
                        }
                        else
                        {
                            throw new Error("Entrada invalida: Solo se permiten numeros del 0 al 9.", log, linea, columna);
                        }
                    }
                    else
                    {
                        match("ReadLine");
                        match("(");


                        string? lineaLeida = Console.ReadLine();
                        if (!float.TryParse(lineaLeida, out float numero))
                        {
                            throw new Error("Entrada invalida: Solo se permiten numeros enteros.", log, linea, columna);
                        }
                        s.Push(numero);
                        v?.setValor(numero, maxTipo);
                    }
                    match(")");
                }
                else
                {
                    Expresion();
                    r = s.Pop();
                    v.setValor(r, maxTipo);
                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                r = v.getValor() + s.Pop();
                v.setValor(r);
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                r = v.getValor() - s.Pop();
                v.setValor(r);
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                r = v.getValor() * s.Pop();
                v.setValor(r);
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                r = v.getValor() / s.Pop();
                v.setValor(r);
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                r = v.getValor() % s.Pop();
                v.setValor(r);
            }
            //displayStack();
        }
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");
            bool ejecuta = Condicion() && ejecuta2;
            //Console.WriteLine(ejecuta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = !ejecuta && ejecuta2; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            maxTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();
            string operador = Contenido;
            match(Tipos.OperadorRelacional);
            maxTipo = Variable.TipoDato.Char;
            Expresion();
            float valor2 = s.Pop();
            switch (operador)
            {
                case ">": return valor1 > valor2;
                case ">=": return valor1 >= valor2;
                case "<": return valor1 < valor2;
                case "<=": return valor1 <= valor2;
                case "==": return valor1 == valor2;
                default: return valor1 != valor2;
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecuta)
        {
            int charTMP = charCount - 6; // Se resta 6 porque se lee el while
            int lineaTMP = linea;
            bool ejecutaWhile;

            do
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(charTMP, SeekOrigin.Begin);
                charCount = charTMP;
                linea = lineaTMP;
                nextToken();

                match("while");
                match("(");

                ejecutaWhile = Condicion() && ejecuta;

                match(")");

                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutaWhile);
                }
                else
                {
                    Instruccion(ejecutaWhile);
                }
            } while (ejecutaWhile);
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool ejecuta)
        {
            //match("do");
            int charTMP = charCount - 3; // Se resta 3 porque se lee el do
            int lineaTMP = linea;
            bool ejecutaDo;
            do
            {
                match("do");
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecuta);
                }
                else
                {
                    Instruccion(ejecuta);
                }
                match("while");
                match("(");
                ejecutaDo = Condicion() && ejecuta;
                match(")");
                match(";");
                if (ejecutaDo)
                {
                    //Seek
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(charTMP, SeekOrigin.Begin);
                    charCount = charTMP;
                    linea = lineaTMP;
                    nextToken();
                }

            } while (ejecutaDo);
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        /*private void For(bool ejecuta)
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            bool ejecutaFor = Condicion() && ejecuta;
            match(";");
            Asignacion();
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecutaFor);
            }
            else
            {
                Instruccion(ejecutaFor);
            }
        }*/
        // En tu clase Lenguaje.cs o donde tengas implementado tu método For:
        private void For(bool ejecuta)
        {
            // Guardar posición inicial
            int initialChar = charCount - 4;
            int initialLine = linea;

            match("for");
            match("(");
            Asignacion(); // Ejecuta la inicialización
            match(";");

            // Guardar posición de la condición
            int condChar = charCount - 2;
            int condLine = linea;

            // Evaluar condición inicial (usando ejecuta)
            bool condition = Condicion() && ejecuta;
            match(";");

            // Guardar posición del incremento (sin ejecutarlo aún)
            int incrChar = charCount - 2;
            int incrLine = linea;

            while (Contenido != ")")
            {
                nextToken();
            }
            match(")"); // Avanza al ")" sin procesar el incremento

            // Guardar posición del bloque
            int blockChar = charCount - 2;
            int blockLine = linea;

            if (condition)
            {
                // Primera iteración del bloque
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(blockChar, SeekOrigin.Begin);
                charCount = blockChar;
                linea = blockLine;
                nextToken();
                if (Contenido == "{") BloqueInstrucciones(true);
                else Instruccion(true);

                while (condition)
                {
                    // Ejecutar el incremento después del bloque
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(incrChar, SeekOrigin.Begin);
                    charCount = incrChar;
                    linea = incrLine;
                    nextToken();
                    Asignacion();
                    match(")");

                    // Re-evaluar condición
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(condChar, SeekOrigin.Begin);
                    charCount = condChar;
                    linea = condLine;
                    nextToken();
                    condition = Condicion() && ejecuta;
                    match(";");

                    // Ejecutar el bloque si la condición es verdadera
                    if (condition)
                    {
                        archivo.DiscardBufferedData();
                        archivo.BaseStream.Seek(blockChar, SeekOrigin.Begin);
                        charCount = blockChar;
                        linea = blockLine;
                        nextToken();
                        if (Contenido == "{")
                        {
                            BloqueInstrucciones(true);
                        }
                        else
                        {
                            Instruccion(true);
                        }
                    }
                }
            }

            // Si la condición inicial es falsa, analizar sintaxis sin ejecutar
            if (!condition)
            {
                archivo.DiscardBufferedData();
                archivo.BaseStream.Seek(blockChar, SeekOrigin.Begin);
                charCount = blockChar;
                linea = blockLine;
                nextToken();
                if (Contenido == "{")
                {
                    BloqueInstrucciones(false);
                }
                else
                {
                    Instruccion(false);
                }
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
        private void console(bool ejecuta)
        {
            bool isWriteLine = false;
            match("Console");
            match(".");

            if (Contenido == "WriteLine")
            {
                match("WriteLine");
                isWriteLine = true;
            }
            else
            {
                match("Write");
            }

            match("(");

            string concatenaciones = "";

            if (Clasificacion == Tipos.Cadena)
            {
                concatenaciones = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            else
            {
                Variable? v = l.Find(var => var.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
                }
                else
                {
                    concatenaciones = v.getValor().ToString();
                    match(Tipos.Identificador);
                }
            }

            if (Contenido == "+")
            {
                match("+");
                concatenaciones += Concatenaciones();  // Se acumula el resultado de las concatenaciones
            }

            match(")");
            match(";");
            if (ejecuta)
            {
                if (isWriteLine)
                {
                    Console.WriteLine(concatenaciones);
                }
                else
                {
                    Console.Write(concatenaciones);
                }
            }
        }
        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v != null)
                {
                    resultado = v.getValor().ToString(); // Obtener el valor de la variable y convertirla
                }
                else
                {
                    throw new Error("La variable " + Contenido + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                resultado = Contenido.Trim('"');
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                match("+");
                resultado += Concatenaciones();  // Acumula el siguiente fragmento de concatenación
            }
            return resultado;
        }
        //Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador)

                {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                float n2 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)

        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                // Si el tipo de dato del número es mayor al tipo de dato actual, cambiarlo
                if (maxTipo < Variable.valorToTipoDato(float.Parse(Contenido)))
                {
                    maxTipo = Variable.valorToTipoDato(float.Parse(Contenido));
                }

                s.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);

                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida ", log, linea, columna);
                }

                if (maxTipo < v.getTipoDato())
                {
                    maxTipo = v.getTipoDato();
                }

                s.Push(v.getValor());
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.FuncionMatematica)
            {
                string nombreFuncion = Contenido;
                match(Tipos.FuncionMatematica);
                match("(");
                Expresion();
                match(")");
                float resultado = s.Pop();
                float mathResult = funcionMatematica(nombreFuncion, resultado, 0);
                s.Push(mathResult);
            }
            else
            {
                match("(");

                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                bool huboCasteo = false;

                // Verificar si hay un tipo de dato explícito (casteo)
                if (Clasificacion == Tipos.TipoDato)
                {
                    switch (Contenido)
                    {
                        case "int": tipoCasteo = Variable.TipoDato.Int; break;
                        case "float": tipoCasteo = Variable.TipoDato.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;
                }

                Expresion();

                if (huboCasteo)
                {
                    float valor = s.Pop(); // Obtener el valor actual del stack
                    switch (tipoCasteo)
                    {
                        case Variable.TipoDato.Int:
                            valor %= 65536; //  2^16
                            break;
                        case Variable.TipoDato.Char:
                            valor %= 256;
                            break;
                    }
                    s.Push(valor); // Regresar el valor casteado al stack
                    maxTipo = tipoCasteo; // Actualizar el tipo máximo
                }
                match(")");
            }
        }
        float resultado;
        public float funcionMatematica(string nombre, float valor, float valor2 = 0)
        {
            float resultado = 0;

            switch (nombre)
            {
                case "abs": resultado = Math.Abs(valor); break;
                case "ceiling": resultado = (float)Math.Ceiling(valor); break;
                case "pow": resultado = (float)Math.Pow(valor, 2); break;
                case "sqrt": resultado = (float)Math.Sqrt(valor); break;
                case "exp": resultado = (float)Math.Exp(valor); break;
                case "equals": resultado = (valor == valor2) ? 1 : 0; break;
                case "floor": resultado = (float)Math.Floor(valor); break;
                case "max": resultado = Math.Max(valor, valor2); break;
                case "min": resultado = Math.Min(valor, valor2); break;
                case "log10": resultado = (float)Math.Log10(valor); break;
                case "log2": resultado = (float)Math.Log2(valor); break;
                case "rand": resultado = (float)new Random().NextDouble(); break;
                case "trunc": resultado = (float)Math.Truncate(valor); break;
                case "round": resultado = (float)Math.Round(valor); break;
            }

            return resultado;
        }




    }
}