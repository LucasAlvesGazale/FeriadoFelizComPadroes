using System;
using System.Collections.Generic;

namespace ProjetoCafeteria
{
    // INTERFACE BASE
    public interface IBebida
    {
        string GetDescricao();
        decimal GetPreco();
    }

    // BEBIDAS
    public class Cafe : IBebida
    {
        public string GetDescricao() => "Café";
        public decimal GetPreco() => 5.00m;
    }

    public class Cha : IBebida
    {
        public string GetDescricao() => "Chá";
        public decimal GetPreco() => 4.00m;
    }

    public class Cappuccino : IBebida
    {
        public string GetDescricao() => "Cappuccino";
        public decimal GetPreco() => 8.50m;
    }

    // PROXY
    public class CappuccinoProxy : IBebida
    {
        private Cappuccino cappuccino;
        private bool usuarioPremium;

        public CappuccinoProxy(bool usuarioPremium)
        {
            this.usuarioPremium = usuarioPremium;
        }

        public string GetDescricao()
        {
            if (!usuarioPremium)
                return "Acesso negado ao Cappuccino";

            GarantirInstancia();
            return cappuccino.GetDescricao();
        }

        public decimal GetPreco()
        {
            if (!usuarioPremium)
                return 0;

            GarantirInstancia();
            return cappuccino.GetPreco();
        }

        private void GarantirInstancia()
        {
            if (cappuccino == null)
                cappuccino = new Cappuccino();
        }
    }

    // FACTORY
    public enum TipoBebida
    {
        Cafe,
        Cha,
        Cappuccino
    }

    public static class BebidaFactory
    {
        public static IBebida CriarBebida(TipoBebida tipo, bool premium = false)
        {
            switch (tipo)
            {
                case TipoBebida.Cafe:
                    return new Cafe();

                case TipoBebida.Cha:
                    return new Cha();

                case TipoBebida.Cappuccino:
                    return new CappuccinoProxy(premium);

                default:
                    throw new ArgumentException("Tipo inválido");
            }
        }
    }

    // ADAPTER
    public class SistemaPagamentoExterno
    {
        public void RealizarPagamento(double valor)
        {
            Console.WriteLine("Pagamento externo: R$ " + valor);
        }
    }

    public interface IPagamento
    {
        void Pagar(decimal valor);
    }

    public class PagamentoAdapter : IPagamento
    {
        private SistemaPagamentoExterno sistema = new SistemaPagamentoExterno();

        public void Pagar(decimal valor)
        {
            sistema.RealizarPagamento((double)valor);
        }
    }

    // SINGLETON
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
                        instancia = new SistemaCafeteria();

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
            Console.WriteLine("Pedidos:");
            foreach (var b in pedidos)
            {
                Console.WriteLine("- " + b.GetDescricao() + " | R$ " + b.GetPreco());
            }
        }

        public void FinalizarPedidos(IPagamento pagamento)
        {
            decimal total = 0;

            foreach (var b in pedidos)
            {
                total += b.GetPreco();
            }

            Console.WriteLine("Total: R$ " + total);
            pagamento.Pagar(total);
        }
    }

    // MAIN
    class Program
    {
        static void Main(string[] args)
        {
            var sistema = SistemaCafeteria.Instancia;

            var b1 = BebidaFactory.CriarBebida(TipoBebida.Cafe);
            var b2 = BebidaFactory.CriarBebida(TipoBebida.Cappuccino, false);
            var b3 = BebidaFactory.CriarBebida(TipoBebida.Cappuccino, true);

            sistema.AdicionarPedido(b1);
            sistema.AdicionarPedido(b2);
            sistema.AdicionarPedido(b3);

            sistema.MostrarPedidos();

            IPagamento pagamento = new PagamentoAdapter();
            sistema.FinalizarPedidos(pagamento);
        }
    }
}
