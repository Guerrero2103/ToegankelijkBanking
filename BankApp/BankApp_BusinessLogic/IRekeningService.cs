using BankApp_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApp_BusinessLogic
{
    public interface IRekeningService
    {
        Task<Rekening> GetRekeningByIdAsync(int rekeningId);
        Task<Rekening> GetRekeningByIbanAsync(string iban);
        Task<List<Rekening>> GetRekeningenByGebruikerIdAsync(int gebruikerId);
        Task<Rekening> MaakRekeningAanAsync(int gebruikerId, RekeningType type);
        Task<bool> UpdateSaldoAsync(int rekeningId, decimal nieuwSaldo);
        Task<decimal> GetTotaalSaldoAsync(int gebruikerId);
    }
}