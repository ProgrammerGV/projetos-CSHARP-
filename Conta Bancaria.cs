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

