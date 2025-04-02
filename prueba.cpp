using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
    int i,n;

    // Leemos n
    Console.Write("Ingresa un número para n: ");
    n = Console.ReadLine();

    // For clásico
    for(i = 0; i < n; i++)
    {
        Console.WriteLine("Valor de i = " + i);
    }
}
