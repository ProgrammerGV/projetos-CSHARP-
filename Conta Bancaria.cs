// Atividade sem nota dia 04/02/2026 
//namespace projetoContaBancaria_Sprint_01
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Digite o seu nome: ");
//            string? nome = Console.ReadLine();
//            if (!string.IsNullOrEmpty(nome))
//            {
//                Console.WriteLine($"Seja bem-vindo(a), {nome}!");
//            }
//            else
//            {
//                Console.WriteLine("Nome inválido. Por favor, reinicie o programa e insira um nome válido.");
//                return;
//            }

//            Console.WriteLine("Sua dívida atual é de R$ 5000,00.");
//            Console.WriteLine("Digite o valor que deseja pagar para quitar sua dívida: ");
//            string? input = Console.ReadLine();

//            if (decimal.TryParse(input, out decimal pagamento))
//            {
//                decimal divida = 5000m;

//                if (pagamento < divida)
//                {
//                    decimal faltadivida = divida - pagamento;
//                    Console.WriteLine($"{nome}, o valor pago foi de: {pagamento:C}");
//                    Console.WriteLine($"A sua dívida restante é de: {faltadivida:C}");
//                }
//                else if (pagamento >= divida)
//                {
//                    Console.WriteLine($"{nome}, o valor pago foi de: {pagamento:C}");
//                    Console.WriteLine("Parabéns! Sua dívida foi totalmente quitada.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("Valor inválido. Por favor, insira um número válido.");
//            }
//        }
//    }
//} Fim da atividade;


// Sprint 01    
public abstract class ContaBancaria
{
    public int NumeroConta { get; private set; }
    public string Titular { get; private set; }
    public decimal Saldo { get; protected set; }
    public string TipoConta { get; protected set; }

    public ContaBancaria(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
    {
        NumeroConta = numeroConta;
        Titular = titular;
        Saldo = saldoInicial;
        TipoConta = tipoConta;
    }
    protected virtual bool PodeSacar(decimal valor)
    {
        return Saldo >= valor;
    }
    public virtual void Sacar(decimal valor)
    {
        if (!PodeSacar(valor))
        {
            throw new InvalidOperationException("Saldo insuficiente para saque.");
        }
        Saldo -= valor;
    }

    public virtual void Depositar(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("O valor do depósito deve ser positivo.");
        }
        Saldo += valor;
    }

    public virtual void SetNumeroConta(int numero)
    {
        NumeroConta = numero;
    }

    public virtual void SetTitular(string titular)
    {
        Titular = titular;
    }

    public virtual void SetTipoConta(string tipo)
    {
        TipoConta = tipo;
    }

}

public class ContaCorrente : ContaBancaria
{
    public ContaCorrente(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta)
    { }
    protected override bool PodeSacar(decimal valor)
    {
        return Saldo >= valor + 5.00m;
    }
    public override void Sacar(decimal valor)
    {
        if (!PodeSacar(valor))
        {
            throw new InvalidOperationException("Saldo insuficiente para saque, considerando a taxa de saque.");
        }
        Saldo -= valor + 5.00m;
    }
}

public class ContaPoupanca : ContaBancaria
{
    public ContaPoupanca(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta)
    { }
    public void AplicarRendimento(decimal taxa)
    {
        Saldo += Saldo * taxa;
    }
}

public class ContaEmpresarial : ContaBancaria
{
    public ContaEmpresarial(int numeroConta, string titular, decimal saldoInicial, string tipoConta)
        : base(numeroConta, titular, saldoInicial, tipoConta)
    {
        LimiteEmprestimo = 1000.00m;
    }
    public decimal LimiteEmprestimo { get; private set; }
    public decimal LimiteEmprestimoEmUso { get; private set; }
    public void DefinirLimiteEmprestimo(decimal limite)
    {
        LimiteEmprestimo = limite;
    }
    public void SolicitarEmprestimo(decimal valor)
    {
        if (valor > (LimiteEmprestimo - LimiteEmprestimoEmUso))
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
        List<ContaBancaria> listaDeContas = new List<ContaBancaria>();
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
                    { 
                    Console.WriteLine("--- Acesso à Conta Corrente ---");

                    Console.Write("Digite o nome do Titular: ");
                    string? nomeCorrente = Console.ReadLine();

                    Console.Write("Digite o número da conta: ");
                    if (int.TryParse(Console.ReadLine(), out int numeroCorrente))
                    {
                        ContaCorrente? contaEncontrada = null;

                        foreach (var conta in listaDeContas)
                        {
                            if (conta is ContaCorrente &&
                                conta.Titular == nomeCorrente &&
                                conta.NumeroConta == numeroCorrente)
                            {
                                contaEncontrada = (ContaCorrente)conta;
                                break;
                            }
                        }

                        if (contaEncontrada != null)
                        {
                            string? opSubMenu;
                            do
                            {
                                Console.Clear();
                                Console.WriteLine($"\n--- Conta: {contaEncontrada.NumeroConta} | Titular: {contaEncontrada.Titular} ---");
                                Console.WriteLine($"Saldo Atual: {contaEncontrada.Saldo:C}");
                                Console.WriteLine("1 - Sacar");
                                Console.WriteLine("2 - Depositar");
                                Console.WriteLine("3 - Voltar ao menu principal");
                                Console.Write("Opção: ");
                                opSubMenu = Console.ReadLine();
                                Console.Clear();

                                    if (opSubMenu == "1")
                                {
                                    Console.Write("Digite o valor para saque: ");
                                    if (decimal.TryParse(Console.ReadLine(), out decimal valorSaque))
                                    {
                                        try
                                        {
                                            contaEncontrada.Sacar(valorSaque);

                                            Console.WriteLine($" Saque de {valorSaque:C} realizado com sucesso!");
                                            Console.WriteLine($" Taxa de saque de R$ 5,00 foi debitada.");
                                            Console.WriteLine($" Novo saldo: {contaEncontrada.Saldo:C}");
                                        }
                                        catch (InvalidOperationException ex)
                                        {
                                            Console.WriteLine($"Erro no saque: {ex.Message}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Valor inválido.");
                                    }
                                }
                                else if (opSubMenu == "2")
                                {
                                    Console.Write("Digite o valor do deposito a ser efetuado: ");
                                    if (decimal.TryParse(Console.ReadLine(), out decimal valorDeposito))
                                    {
                                        try
                                        {
                                            contaEncontrada.Depositar(valorDeposito);
                                            Console.WriteLine($" Depósito de {valorDeposito:C} realizado com sucesso!");
                                            Console.WriteLine($" Novo saldo: {contaEncontrada.Saldo:C}");
                                        }
                                        catch (ArgumentException ex)
                                        {
                                            Console.WriteLine($"Erro no depósito: {ex.Message}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Valor inválido.");
                                    }
                                }

                            } while (opSubMenu != "3");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(" Erro: Conta não encontrada! Verifique o nome e o número.");
                        }
                    }
                    else
                    {
                            Console.Clear();
                            Console.WriteLine("Número de conta inválido.");
                    }
            }
                        break;
                case "2":
                    {
                        Console.WriteLine("--- Acesso à Conta Poupança ---");
                        Console.Write("Digite o nome do Titular: ");
                        string? nomePoupanca = Console.ReadLine();

                        Console.Write("Digite o número da conta: ");
                        if (int.TryParse(Console.ReadLine(), out int numeroPoupanca))
                        {
                            ContaPoupanca? contaEncontrada = null;

                            foreach (var conta in listaDeContas)
                            {
                                if (conta is ContaPoupanca &&
                                    conta.Titular == nomePoupanca &&
                                    conta.NumeroConta == numeroPoupanca)
                                {
                                    contaEncontrada = (ContaPoupanca)conta; 
                                    break;
                                }
                            }

                            if (contaEncontrada != null)
                            {
                                string? opSubMenu;
                                do
                                {
                                    Console.WriteLine($"\n--- Conta Poupança: {contaEncontrada.NumeroConta} | Titular: {contaEncontrada.Titular} ---");
                                    Console.WriteLine($"Saldo Atual: {contaEncontrada.Saldo:C}");
                                    Console.WriteLine("1 - Sacar");
                                    Console.WriteLine("2 - Depositar");
                                    Console.WriteLine("3 - Aplicar Rendimento"); 
                                    Console.WriteLine("4 - Voltar ao menu principal");
                                    Console.Write("Opção: ");
                                    opSubMenu = Console.ReadLine();

                                    if (opSubMenu == "1")
                                    {
                                        Console.Write("Valor do saque: ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal valor))
                                        {
                                            try
                                            {
                                                contaEncontrada.Sacar(valor);
                                                Console.WriteLine($"Saque realizado! Novo saldo: {contaEncontrada.Saldo:C}");
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                    else if (opSubMenu == "2")
                                    {
                                        Console.Write("Valor do depósito: ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal valor))
                                        {
                                            try
                                            {
                                                contaEncontrada.Depositar(valor);
                                                Console.WriteLine($"Depósito realizado! Novo saldo: {contaEncontrada.Saldo:C}");
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                    else if (opSubMenu == "3")
                                    {
                                        Console.Write("Digite a taxa de rendimento (ex: 0,05 para 5%): ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal taxa))
                                        {
                                            contaEncontrada.AplicarRendimento(taxa);
                                            Console.WriteLine($"Rendimento aplicado! Novo saldo: {contaEncontrada.Saldo:C}");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Taxa inválida.");
                                        }
                                    }

                                } while (opSubMenu != "4");
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine(" Erro: Conta Poupança não encontrada.");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Número inválido.");
                        }
                    }
                    break;

                case "3":
                    {
                        Console.WriteLine("--- Acesso à Conta Empresarial ---");
                        Console.Write("Digite o nome do titular: ");
                        string? nomeEmpresarial = Console.ReadLine();

                        Console.Write("Digite o número da conta: ");
                        if (int.TryParse(Console.ReadLine(), out int numeroEmpresarial))
                        {
                            ContaEmpresarial? contaEncontrada = null;
                            foreach (var conta in listaDeContas)
                            {
                                if (conta is ContaEmpresarial &&
                                    conta.Titular == nomeEmpresarial &&
                                    conta.NumeroConta == numeroEmpresarial)
                                {
                                    contaEncontrada = (ContaEmpresarial)conta;
                                    break;
                                }
                            }
                            if (contaEncontrada != null)
                            {
                                string? opSubMenu;
                                do
                                {
                                    Console.WriteLine($"\n--- Conta Empresarial: {contaEncontrada.NumeroConta} | Titular: {contaEncontrada.Titular} ---");
                                    Console.WriteLine($"Saldo Atual: {contaEncontrada.Saldo:C}");
                                    decimal disponivel = contaEncontrada.LimiteEmprestimo - contaEncontrada.LimiteEmprestimoEmUso;
                                    Console.WriteLine($"Limite para Empréstimo: {disponivel:C}");
                                    Console.WriteLine("1 - Sacar");
                                    Console.WriteLine("2 - Depositar");
                                    Console.WriteLine("3 - Solicitar Empréstimo");
                                    Console.WriteLine("4 - Voltar ao menu principal");
                                    Console.Write("Opção: ");
                                    opSubMenu = Console.ReadLine();

                                    if (opSubMenu == "1")
                                    {
                                        Console.Write("Valor do Saque: ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal valorSaque))
                                        {
                                            try
                                            {
                                                contaEncontrada.Sacar(valorSaque);
                                                Console.WriteLine($"Saque realizado! Novo saldo: {contaEncontrada.Saldo:C}");
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                    else if (opSubMenu == "2")
                                    {
                                        Console.Write("Valor do Depósito: ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal valorDeposito))
                                        {
                                            try
                                            {
                                                contaEncontrada.Depositar(valorDeposito);
                                                Console.WriteLine($"Depósito realizado! Novo saldo: {contaEncontrada.Saldo:C}");
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                    else if (opSubMenu == "3")
                                    {
                                        Console.Write("Valor do Empréstimo: ");
                                        if (decimal.TryParse(Console.ReadLine(), out decimal valorEmprestimo))
                                        {
                                            try
                                            {
                                                contaEncontrada.SolicitarEmprestimo(valorEmprestimo);
                                                Console.WriteLine($"Empréstimo aprovado! Novo saldo: {contaEncontrada.Saldo:C}");
                                            }
                                            catch (Exception ex) { Console.WriteLine(ex.Message); }
                                        }
                                    }
                                } while (opSubMenu != "4");
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine(" Erro: Conta Empresarial não encontrada.");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Número de conta inválido.");
                        }
                    }
                    break;

                    case "4":

                    Console.WriteLine("1 - Conta Corrente");
                    Console.WriteLine("2 - Conta Poupança");
                    Console.WriteLine("3 - Conta Empresarial");
                    Console.Write("Selecione o tipo de conta: ");
                    string? tipoOpcao = Console.ReadLine();
                    Console.Clear();
                    Console.Write("Digite seu nome: ");
                    string? nomeTitular = Console.ReadLine();

                    string tipoTexto = "";
                    if (tipoOpcao == "1") tipoTexto = "Conta Corrente";
                    else if (tipoOpcao == "2") tipoTexto = "Conta Poupança";
                    else if (tipoOpcao == "3") tipoTexto = "Conta Empresarial";
                    else
                    {
                        Console.WriteLine("Tipo inválido!");
                        break; 
                    }

                    bool contaJaExiste = false;

                    for (int i = 0; i < listaDeContas.Count; i++)
                    {
                        if (listaDeContas[i].Titular == nomeTitular && listaDeContas[i].TipoConta == tipoTexto)
                        {
                            contaJaExiste = true;
                            break; 
                        }
                    }

                    if (contaJaExiste)
                    {
                        Console.WriteLine($"Erro: O cliente {nomeTitular} já possui uma {tipoTexto}.");
                    }
                    else
                    {
                        int novoNumero = listaDeContas.Count + 1;
                        if (tipoOpcao == "1")
                        {
                            ContaBancaria novaConta = new ContaCorrente(novoNumero, nomeTitular!, 0m, tipoTexto);
                            listaDeContas.Add(novaConta);
                        }
                        else if (tipoOpcao == "2")
                        {
                            ContaBancaria novaConta = new ContaPoupanca(novoNumero, nomeTitular!, 0m, tipoTexto);
                            listaDeContas.Add(novaConta);
                        }
                        else if (tipoOpcao == "3")
                        {
                            ContaBancaria novaConta = new ContaEmpresarial(novoNumero, nomeTitular!, 0m, tipoTexto);
                            listaDeContas.Add(novaConta);
                        }

                        Console.WriteLine($"{tipoTexto} criada com sucesso! Número da conta: {novoNumero}\n");
                    }

                    break;

                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                    }
        } while (opcao != "0");

    }
}