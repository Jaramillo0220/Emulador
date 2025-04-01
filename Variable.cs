using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emulador
{
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        }
        TipoDato tipo;
        string nombre;
        float valor;
        public Variable(TipoDato tipo, string nombre, float valor = 0)
        {
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }
        public void setValor(float valor, TipoDato maximoTipo)
        {
            if (maximoTipo <= tipo)
            {
                this.valor = valor;
            }
            else
            {
                Console.WriteLine("Este se esta ejecutando");
                throw new Error("Semántico: no se puede asignar un " + maximoTipo + " a un " + tipo + " en: [" + Lexico.linea + "," + Lexico.columna + "]");
            }
        }
        public void setValor(float valor)
        {
            if (valorToTipoDato(valor) <= tipo)
            {
                this.valor = valor;
            }
            else
            {
                throw new Error("Semántico: no se puede asignar un " + valorToTipoDato(valor) + " a un " + tipo + " en: [" + Lexico.linea + "," + Lexico.columna + "]");
            }
        }


        public static TipoDato valorToTipoDato(float valor)
        {
            if (Math.Floor(valor) != valor)
            {
                return TipoDato.Float;
            }
            else if (valor <= 255)
            {
                return TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return TipoDato.Int;
            }
            else
            {
                return TipoDato.Float;
            }
        }

        public float getValor()
        {
            return valor;
        }
        public string getNombre()
        {
            return nombre;
        }
        public TipoDato getTipoDato()
        {
            return tipo;
        }
    }
}