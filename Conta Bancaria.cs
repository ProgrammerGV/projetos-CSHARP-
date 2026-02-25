using System;

public interface ITransacao
{
    void Sacar(decimal valor);
    void Depositar(decimal valor);
}

public abstract class ContaBancaria : ITransacao
{
    public int NumeroConta { get; private set; }
    public string Titular { get; private set; }
    public decimal Saldo { get; protected set; }
    public string TipoConta { get; protected set; }

    public int CountCorrente { get; private set; }
    public int CountPoupanca { get; private set; }
    public int CountEmpresarial { get; private set; }

    protected ContaBancaria(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
    {
        NumeroConta = numeroConta;
        Titular = titular;
        Saldo = saldoInicial;
        TipoConta = tipoConta;
    }

    protected ContaBancaria() 
    {
        Titular = string.Empty;
        TipoConta = string.Empty;
    }
    public void IncrementarContador(string tipoConta)
    {
        if (tipoConta == "1") CountCorrente++;
        else if (tipoConta == "2") CountPoupanca++;
        else if (tipoConta == "3") CountEmpresarial++;
    }

    public int ObterContador(string tipoConta)
    {
        return tipoConta switch
        {
            "1" => CountCorrente,
            "2" => CountPoupanca,
            "3" => CountEmpresarial,
            _ => 0
        };
    }

    protected virtual bool PodeSacar(decimal valor)
    {
        if ( valor <= 0)
        {
            return false;
        }
        return Saldo >= valor;
    }

    public virtual void Sacar(decimal valor)
    {
        if (!PodeSacar(valor))
            throw new InvalidOperationException("Saldo insuficiente para saque.");
        Saldo -= valor;
    }

    public virtual void Depositar(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor do depósito deve ser positivo.");
        Saldo += valor;
    }

    public void SetNumeroConta(int numero) => NumeroConta = numero;
    public void SetTitular(string titular) => Titular = titular;
    public void SetTipoConta(string tipo) => TipoConta = tipo;
}

public class ContaCorrente : ContaBancaria
{
    public ContaCorrente(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta) { }
    protected override bool PodeSacar(decimal valor)
    {
        if (valor <= 0) {
            return false;
            }
        return Saldo >= (valor + 5.00m);
    }
    public override void Sacar(decimal valor)
    {
        if (!PodeSacar(valor))
            throw new InvalidOperationException("Saldo insuficiente para saque (incluindo taxa de R$ 5,00).");
        Saldo -= (valor + 5.00m);
    }
}

public class ContaPoupanca : ContaBancaria
{
    public ContaPoupanca(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta) { }

    public void AplicarRendimento(decimal taxa)
    {
        if (taxa <= 0 || taxa > 1)
        {
            throw new ArgumentException("A taxa de rendimento deve ser maior que 0 e no máximo 1 (100%).");
        }

        Saldo += Saldo * taxa;
    }
}

public class ContaEmpresarial : ContaBancaria
{
    public decimal LimiteEmprestimo { get; private set; } = 1000.00m;
    public decimal LimiteEmprestimoEmUso { get; private set; }

    public ContaEmpresarial(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta) { }

    public void DefinirLimiteEmprestimo(decimal limite) => LimiteEmprestimo = limite;

    public void SolicitarEmprestimo(decimal valor)
    {
if (valor <= 0)
        throw new ArgumentException("O valor do empréstimo deve ser maior que zero.");

    if (valor > (LimiteEmprestimo - LimiteEmprestimoEmUso))
        throw new InvalidOperationException("Valor do empréstimo excede o limite disponível.");

    LimiteEmprestimoEmUso += valor;
    Saldo += valor;
    }
}

class Program
{
    static List<ContaBancaria> listaDeContas = new List<ContaBancaria>();

    static void Main(string[] args)
    {
        string? opcao;
        do
        {
            Console.WriteLine("1 - Conta Corrente");
            Console.WriteLine("2 - Conta Poupança");
            Console.WriteLine("3 - Conta Empresarial");
            Console.WriteLine("4 - Abrir conta");
            Console.WriteLine("0 - Sair");
            Console.Write("\nEscolha uma opção: ");
            opcao = Console.ReadLine();
            Console.Clear();

            switch (opcao)
            {
                case "0":
                    Console.WriteLine("Saindo...");
                    break;

                case "1":
                    ProcessarAcessoConta("Conta Corrente");
                    break;

                case "2":
                    ProcessarAcessoConta("Conta Poupança");
                    break;

                case "3":
                    ProcessarAcessoConta("Conta Empresarial");
                    break;

                case "4":
                    ProcessarAberturaConta();
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

        } while (opcao != "0");
    }

    static void ProcessarAcessoConta(string tipoContaDescricao)
    {
        Console.WriteLine($"--- Acesso à {tipoContaDescricao} ---");
        Console.Write("Digite o nome do Titular: ");
        string? nome = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.Clear();
            Console.WriteLine("Nome inválido.");
            return;
        }

        Console.Write("Digite o número da conta: ");
        if (int.TryParse(Console.ReadLine(), out int numero))
        {
            var contaEncontrada = listaDeContas.Find(c =>
                c.Titular.Equals(nome?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                c.NumeroConta == numero &&
                ((tipoContaDescricao == "Conta Corrente" && c is ContaCorrente) ||
                 (tipoContaDescricao == "Conta Poupança" && c is ContaPoupanca) ||
                 (tipoContaDescricao == "Conta Empresarial" && c is ContaEmpresarial))
            );

            if (contaEncontrada != null)
            {
                MenuOperacoes(contaEncontrada, tipoContaDescricao);
            }
            else
            {
                Console.WriteLine($"\nErro: {tipoContaDescricao} não encontrada! Verifique o nome e o número.");
                Thread.Sleep(2000); 
                Console.Clear();
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Número de conta inválido.");
        }
    }

    static void MenuOperacoes(ContaBancaria conta, string tipoContaDescricao)
    {
        string? opSubMenu;
        do
        {
            Console.Clear();
            Console.WriteLine($"\n--- {tipoContaDescricao}: {conta.NumeroConta} | Titular: {conta.Titular} ---");
            Console.WriteLine($"Saldo Atual: {conta.Saldo:C}");

            if (conta is ContaEmpresarial ce)
            {
                decimal disponivel = ce.LimiteEmprestimo - ce.LimiteEmprestimoEmUso;
                Console.WriteLine($"Limite para Empréstimo: {disponivel:C}");
            }

            Console.WriteLine("1 - Sacar");
            Console.WriteLine("2 - Depositar");

            if (conta is ContaPoupanca)
            {
                Console.WriteLine("3 - Aplicar Rendimento");
                Console.WriteLine("4 - Voltar ao menu principal");
            }
            else if (conta is ContaEmpresarial)
            {
                Console.WriteLine("3 - Solicitar Empréstimo");
                Console.WriteLine("4 - Voltar ao menu principal");
            }
            else 
            {
                Console.WriteLine("3 - Voltar ao menu principal");
            }

            Console.Write("Opção: ");
            opSubMenu = Console.ReadLine();
            Console.Clear();

            try
            {
                if (opSubMenu == "1")
                {
                    RealizarTransacao(conta, "Saque");
                }
                else if (opSubMenu == "2")
                {
                    RealizarTransacao(conta, "Depósito");
                }
                else if (opSubMenu == "3")
                {
                    if (conta is ContaPoupanca cp)
                    {
                        RealizarTransacaoEspecifica(cp, "Rendimento");
                    }
                    else if (conta is ContaEmpresarial cemp)
                    {
                        RealizarTransacaoEspecifica(cemp, "Empréstimo");
                    }
                    else
                    {
                        opSubMenu = "4";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }

        } while (opSubMenu != "4");
    }

    static void RealizarTransacao(ContaBancaria conta, string tipo)
    {
        Console.Write($"Digite o valor para {tipo.ToLower()}: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal valor))
        {
            if (tipo == "Saque")
            {
                conta.Sacar(valor);
                Console.WriteLine($" Saque de {valor:C} realizado com sucesso!");
                if (conta is ContaCorrente) Console.WriteLine($" Taxa de saque de R$ 5,00 foi debitada.");
            }
            else 
            {
                conta.Depositar(valor);
                Console.WriteLine($" Depósito de {valor:C} realizado com sucesso!");
            }
            Console.WriteLine($" Novo saldo: {conta.Saldo:C}");
        }
        else
        {
            Console.WriteLine("Valor inválido.");
        }
    }

    static void RealizarTransacaoEspecifica(ContaBancaria conta, string tipo)
    {
        if (tipo == "Rendimento" && conta is ContaPoupanca cp)
        {
            Console.Write("Digite a taxa de rendimento (ex: 0,05 para 5%): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal taxa))
            {
                cp.AplicarRendimento(taxa);
                Console.WriteLine($"Rendimento aplicado! Novo saldo: {cp.Saldo:C}");
            }
            else Console.WriteLine("Taxa inválida.");
        }
        else if (tipo == "Empréstimo" && conta is ContaEmpresarial ce)
        {
            Console.Write("Valor do Empréstimo: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal valor))
            {
                ce.SolicitarEmprestimo(valor);
                Console.WriteLine($"Empréstimo aprovado! Novo saldo: {ce.Saldo:C}");
            }
            else Console.WriteLine("Valor inválido.");
        }
    }

    static void ProcessarAberturaConta()
    {
        Console.WriteLine("1 - Conta Corrente");
        Console.WriteLine("2 - Conta Poupança");
        Console.WriteLine("3 - Conta Empresarial");
        Console.Write("Selecione o tipo de conta: ");
        string? tipoOpcao = Console.ReadLine();
        Console.Clear();

        string? tipoTexto = tipoOpcao switch
        {
            "1" => "Conta Corrente",
            "2" => "Conta Poupança",
            "3" => "Conta Empresarial",
            _ => null
        };

        if (tipoTexto == null)
        {
            Console.WriteLine("Tipo inválido!");
            return;
        }

        Console.Write("Digite seu nome: ");
        string? nomeTitular = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(nomeTitular))
        {
            Console.WriteLine("Erro: O nome do titular não pode ser vazio.");
            return;
        }

        if (listaDeContas.Exists(c => c.Titular.Equals(nomeTitular, StringComparison.OrdinalIgnoreCase) && c.TipoConta == tipoTexto))
        {
            Console.WriteLine($"Erro: O cliente {nomeTitular} já possui uma {tipoTexto}.");
        }
        else
        {
            int novoNumero = listaDeContas.Count(c => c.TipoConta == tipoTexto) + 1;
            ContaBancaria? novaConta = null;

            if (tipoOpcao == "1")
            { 
                novaConta = new ContaCorrente(novoNumero, nomeTitular!, 0m, tipoTexto); 
            }
            else if (tipoOpcao == "2")
            {
                novaConta = new ContaPoupanca(novoNumero, nomeTitular!, 0m, tipoTexto);
            }
            else if (tipoOpcao == "3")
            {
                novaConta = new ContaEmpresarial(novoNumero, nomeTitular!, 0m, tipoTexto); 
            }

            if (novaConta != null)
            {
                listaDeContas.Add(novaConta);
                Console.WriteLine($"{tipoTexto} criada com sucesso! Número da conta: {novoNumero}\n");
            }
        }
    }
}