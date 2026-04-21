using System;
using System.Collections.Generic;

namespace ProjetoCafeteria
{
    // 1) INTERFACE BASE
    public interface IBebida
    {
        string GetDescricao();
        decimal GetPreco();
    }

    // 2) BEBIDAS CONCRETAS
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

    // 3) DECORATOR
    public abstract class BebidaDecorator : IBebida
    {
        protected IBebida bebida;

        public BebidaDecorator(IBebida bebida)
        {
            this.bebida = bebida;
        }

        public abstract string GetDescricao();
        public abstract decimal GetPreco();
    }

    public class LeiteDecorator : BebidaDecorator
    {
        public LeiteDecorator(IBebida bebida) : base(bebida) { }

        public override string GetDescricao()
        {
            return bebida.GetDescricao() + " + Leite";
        }

        public override decimal GetPreco()
        {
            return bebida.GetPreco() + 1.50m;
        }
    }

    public class ChocolateDecorator : BebidaDecorator
    {
        public ChocolateDecorator(IBebida bebida) : base(bebida) { }

        public override string GetDescricao()
        {
            return bebida.GetDescricao() + " + Chocolate";
        }

        public override decimal GetPreco()
        {
            return bebida.GetPreco() + 2.00m;
        }
    }

    // 4) PROXY
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
                return 0m;

            GarantirInstancia();
            return cappuccino.GetPreco();
        }

        private void GarantirInstancia()
        {
            if (cappuccino == null)
                cappuccino = new Cappuccino();
        }
    }

    // 5) FACTORY
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
                    throw new ArgumentException("Tipo de bebida inválido.");
            }
        }
    }

    // 6) STRATEGY
    public interface IDescontoStrategy
    {
        decimal Aplicar(decimal valor);
        string Nome { get; }
    }

    public class SemDesconto : IDescontoStrategy
    {
        public string Nome => "Sem desconto";

        public decimal Aplicar(decimal valor)
        {
            return valor;
        }
    }

    public class DescontoFidelidade : IDescontoStrategy
    {
        public string Nome => "Desconto fidelidade (10%)";

        public decimal Aplicar(decimal valor)
        {
            return valor * 0.90m;
        }
    }

    public class DescontoPromocional : IDescontoStrategy
    {
        public string Nome => "Desconto promocional (15%)";

        public decimal Aplicar(decimal valor)
        {
            return valor * 0.85m;
        }
    }

    // 7) ADAPTER
    public class SistemaPagamentoExterno
    {
        public void EfetuarPagamento(double valor)
        {
            Console.WriteLine("Pagamento realizado no sistema externo: R$ " + valor.ToString("0.00"));
        }
    }

    public interface IPagamento
    {
        void Pagar(decimal valor);
    }

    public class PagamentoAdapter : IPagamento
    {
        private SistemaPagamentoExterno sistemaExterno = new SistemaPagamentoExterno();

        public void Pagar(decimal valor)
        {
            sistemaExterno.EfetuarPagamento((double)valor);
        }
    }

    // 8) OBSERVER
    public interface IObservadorPedido
    {
        void Atualizar(string mensagem);
    }

    public class ClienteObserver : IObservadorPedido
    {
        private string nome;

        public ClienteObserver(string nome)
        {
            this.nome = nome;
        }

        public void Atualizar(string mensagem)
        {
            Console.WriteLine("Cliente " + nome + " recebeu aviso: " + mensagem);
        }
    }

    public class CozinhaObserver : IObservadorPedido
    {
        public void Atualizar(string mensagem)
        {
            Console.WriteLine("Cozinha recebeu aviso: " + mensagem);
        }
    }

    // 9) SINGLETON
    public class SistemaCafeteria
    {
        private static SistemaCafeteria instancia;
        private static readonly object trava = new object();

        private List<IBebida> pedidos;
        private List<IObservadorPedido> observadores;

        private SistemaCafeteria()
        {
            pedidos = new List<IBebida>();
            observadores = new List<IObservadorPedido>();
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

        public void AdicionarObservador(IObservadorPedido observador)
        {
            observadores.Add(observador);
        }

        public void RemoverObservador(IObservadorPedido observador)
        {
            observadores.Remove(observador);
        }

        public void Notificar(string mensagem)
        {
            foreach (IObservadorPedido obs in observadores)
            {
                obs.Atualizar(mensagem);
            }
        }

        public void AdicionarPedido(IBebida bebida)
        {
            pedidos.Add(bebida);
            Notificar("Novo pedido adicionado: " + bebida.GetDescricao());
        }

        public void MostrarPedidos()
        {
            Console.WriteLine();
            Console.WriteLine("=== PEDIDOS REGISTRADOS ===");

            foreach (IBebida bebida in pedidos)
            {
                Console.WriteLine("- " + bebida.GetDescricao() + " | R$ " + bebida.GetPreco().ToString("0.00"));
            }
        }
    }

    // 10) FACHADA
    public class PedidoFacade
    {
        private SistemaCafeteria sistema = SistemaCafeteria.Instancia;

        public void FazerPedido(
            TipoBebida tipo,
            bool premium,
            List<string> extras,
            IDescontoStrategy desconto,
            IPagamento pagamento)
        {
            IBebida bebida = BebidaFactory.CriarBebida(tipo, premium);

            if (extras != null)
            {
                foreach (string extra in extras)
                {
                    string e = extra.ToLower();

                    if (e == "leite")
                        bebida = new LeiteDecorator(bebida);
                    else if (e == "chocolate")
                        bebida = new ChocolateDecorator(bebida);
                }
            }

            sistema.AdicionarPedido(bebida);

            decimal valorOriginal = bebida.GetPreco();
            decimal valorFinal = desconto.Aplicar(valorOriginal);

            Console.WriteLine();
            Console.WriteLine("Pedido montado: " + bebida.GetDescricao());
            Console.WriteLine("Desconto usado: " + desconto.Nome);
            Console.WriteLine("Valor original: R$ " + valorOriginal.ToString("0.00"));
            Console.WriteLine("Valor final: R$ " + valorFinal.ToString("0.00"));

            pagamento.Pagar(valorFinal);

            sistema.Notificar("Pedido finalizado: " + bebida.GetDescricao());
        }
    }

    // MAIN
    class Program
    {
        static void Main(string[] args)
        {
            SistemaCafeteria sistema = SistemaCafeteria.Instancia;

            sistema.AdicionarObservador(new ClienteObserver("Lucas"));
            sistema.AdicionarObservador(new ClienteObserver("Maria"));
            sistema.AdicionarObservador(new CozinhaObserver());

            PedidoFacade fachada = new PedidoFacade();

            List<string> extras1 = new List<string> { "leite", "chocolate" };
            fachada.FazerPedido(
                TipoBebida.Cappuccino,
                true,
                extras1,
                new DescontoPromocional(),
                new PagamentoAdapter()
            );

            List<string> extras2 = new List<string> { "leite" };
            fachada.FazerPedido(
                TipoBebida.Cafe,
                false,
                extras2,
                new DescontoFidelidade(),
                new PagamentoAdapter()
            );

            sistema.MostrarPedidos();
        }
    }
}
