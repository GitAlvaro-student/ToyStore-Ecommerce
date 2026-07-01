namespace ToyStore.Web.Models;

/// <summary>
/// Dados fictícios do cliente, usados apenas para preencher a tela de Checkout.
/// </summary>
public class Customer
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}
