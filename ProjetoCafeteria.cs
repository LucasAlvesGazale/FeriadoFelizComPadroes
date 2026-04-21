using System;
using System.Collections.Generic;

namespace ProjetoCafeteria
{
    // Interface base para facilitar os próximos padrões, como Decorator
    public interface IBebida
    {
        string GetDescricao();
        decimal GetPreco();
    }

    // Bebidas concretas
    public class Cafe : IBebida
    {
        public string GetDescricao()
        {
            return "Café";
        }

        public decimal GetPreco()
        {
            return 5.00m;
        }
    }

    public class Cha : IBebida
    {
        public string GetDescricao()
        {
            return "Chá";
        }

        public decimal GetPreco()
        {
            return 4.00m;
        }
    }

    public class Cappuccino : IBebida
    {
        public string GetDescricao()
        {
            return "Cappuccino";
        }

        public decimal GetPreco()
        {
            return 8.50m;
        }
    }

    // Factory
    public enum TipoBebida
    {
        Cafe,
        Cha,
        Cappuccino
    }

    public static class BebidaFactory
    {
        public static IBebida CriarBebida(TipoBebida tipo)
        {
            switch (tipo)
            {
                case TipoBebida.Cafe:
                    return new Cafe();

                case TipoBebida.Cha:
                    return new Cha();

                case TipoBebida.Cappuccino:
                    return new Cappuccino();

                default:
                    throw new ArgumentException("Tipo de bebida inválido.");
            }
        }
    }

    // Singleton
    public class SistemaCafeteria
    {
        private static SistemaCafeteria instancia;
        private static readonly object trava = new object();

        private List<IBebida> pedidos;

        private SistemaCafeteria()
        {
            pedidos = new List<IBebida>();
        }

        public static SistemaCafeteria Instancia
        {
            get
            {
                lock (trava)
                {
                    if (instancia == null)
                    {
                        instancia = new SistemaCafeteria();
                    }
                    return instancia;
                }
            }
        }

        public void AdicionarPedido(IBebida bebida)
        {
            pedidos.Add(bebida);
        }

        public void MostrarPedidos()
        {
            Console.WriteLine("Pedidos registrados:");
            foreach (IBebida bebida in pedidos)
            {
                Console.WriteLine("- " + bebida.GetDescricao() + " | R$ " + bebida.GetPreco());
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SistemaCafeteria sistema = SistemaCafeteria.Instancia;

            IBebida bebida1 = BebidaFactory.CriarBebida(TipoBebida.Cafe);
            IBebida bebida2 = BebidaFactory.CriarBebida(TipoBebida.Cappuccino);

            sistema.AdicionarPedido(bebida1);
            sistema.AdicionarPedido(bebida2);

            sistema.MostrarPedidos();
        }
    }
}
