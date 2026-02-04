public abstract class ContaBancaria
{
    public int NumeroConta {  get; private set; }
    public string Titular { get;  private set; }
    public decimal Saldo { get; protected set; }
    public ContaBancaria(int numeroConta, string titular, decimal saldoInicial)
    {
        NumeroConta = numeroConta;
        Titular = titular;
        Saldo = saldoInicial;
    }
    protected virtual bool PodeSacar(decimal valor)
    {
    return Saldo >= valor;
    }
    public virtual void Sacar(decimal valor)
    {
        if(!PodeSacar(valor)){
            throw new InvalidOperationException("Saldo insuficiente para saque.");            
        }
        Saldo -= valor;
    }

}

public class ContaCorrente : ContaBancaria
{
    public ContaCorrente(int numeroConta, string titular, decimal saldoInicial)
        : base(numeroConta, titular, saldoInicial)
    {}
    protected override bool PodeSacar(decimal valor)
    {
        return Saldo >= valor + 5.00m;
    }
    public override void Sacar(decimal valor)
    {
        if(!PodeSacar(valor)){
            throw new InvalidOperationException("Saldo insuficiente para saque, considerando a taxa de saque.");
        }
        Saldo -= valor + 5.00m; 
    }
}

public class ContaPoupanca : ContaBancaria
{
    public ContaPoupanca(int numeroConta, string titular, decimal saldoInicial)
        : base(numeroConta, titular, saldoInicial)
    {}    
    public void AplicarRendimento(decimal taxa)
    {
        Saldo += Saldo * taxa;
    }
}

public class ContaEmpresarial : ContaBancaria
{
    public ContaEmpresarial(int numeroConta, string titular, decimal saldoInicial)
        : base(numeroConta, titular, saldoInicial)
    {}
    public decimal LimiteEmprestimo { get; private set; }
    public decimal LimiteEmprestimoEmUso { get; private set; }
    public void DefinirLimiteEmprestimo(decimal limite)
    {
        LimiteEmprestimo = limite;
    }
    public void SolicitarEmprestimo(decimal valor)
    {
        if(valor > (LimiteEmprestimo - LimiteEmprestimoEmUso))
        {
            throw new InvalidOperationException("Valor do empréstimo excede o limite disponível.");
        }
        LimiteEmprestimoEmUso += valor;
        Saldo += valor;
    }
}

class Program
{
    static void Main(string[] args)
    {
        string opcao;
        do
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Conta Corrente");
            Console.WriteLine("2 - Conta Poupança");
            Console.WriteLine("3 - Conta Empresarial");
            Console.WriteLine("0 - Sair");
            opcao = Console.ReadLine();
            Console.WriteLine();
            try
             {
                switch (opcao)
                {
                    case "1":
                        Console.WriteLine("Você escolheu Conta Corrente.");
                        Console.WriteLine("Digite o nome do Titular: ");
                        string titularCorrente = Console.ReadLine();
                        Console.WriteLine("Digite o numero da conta: ");
                        decimal numeroContaCorrente = decimal.Parse(Console.ReadLine());
                        ContaCorrente contaCorrente = new ContaCorrente(numeroContaCorrente, titularCorrente, 1000.00m);
                        contaCorrente.Sacar(200.00m);
                        Console.WriteLine($"Saldo Conta Corrente: {contaCorrente.Saldo}");
                        break;
                    case "2":
                        Console.WriteLine("Você escolheu Conta Poupança.");
                        Console.WriteLine("Digite o nome do Titular: ");
                        string titularPoupanca = Console.ReadLine();
                        Console.WriteLine("Digite o numero da conta: ");
                       decimal numeroContaPoupanca = decimal.Parse(Console.ReadLine());
                        ContaPoupanca contaPoupanca = new ContaPoupanca(numeroContaPoupanca, titularPoupanca, 1500.00m);
                        contaPoupanca.AplicarRendimento(0.05m);
                        Console.WriteLine($"Saldo Conta Poupança: {contaPoupanca.Saldo}");
                        break;
                    case "3":
                        Console.WriteLine("Você escolheu Conta Empresarial.");
                        Console.WriteLine("Digite o nome do Titular: ");
                        string titularEmpresarial = Console.ReadLine();
                        Console.WriteLine("Digite o numero da conta: ");
                        decimal numeroContaEmpresarial = decimal.Parse(Console.ReadLine());
                        ContaEmpresarial contaEmpresarial = new ContaEmpresarial(numeroContaEmpresarial, titularEmpresarial, 5000.00m);
                        contaEmpresarial.DefinirLimiteEmprestimo(2000.00m);
                        contaEmpresarial.SolicitarEmprestimo(1500.00m);
                        Console.WriteLine($"Saldo Conta Empresarial: {contaEmpresarial.Saldo}");
                        break;
                    case "0":
                        Console.WriteLine("Saindo...");

                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}"); 
        }
        } while (opcao != "0");
   
    }
}