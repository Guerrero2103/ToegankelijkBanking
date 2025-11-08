using BankApp_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp_BusinessLogic
{
    public class RekeningService : IRekeningService
    {
        private readonly AppDbContext _context;

        public RekeningService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rekening> GetRekeningByIdAsync(int rekeningId)
        {
            return await _context.Rekeningen
                .Include(r => r.Gebruiker)
                .FirstOrDefaultAsync(r => r.Id == rekeningId);
        }

        public async Task<Rekening> GetRekeningByIbanAsync(string iban)
        {
            return await _context.Rekeningen
                .Include(r => r.Gebruiker)
                .FirstOrDefaultAsync(r => r.Iban == iban);
        }

        public async Task<List<Rekening>> GetRekeningenByGebruikerIdAsync(int gebruikerId)
        {
            return await _context.Rekeningen
                .Where(r => r.GebruikerId == gebruikerId)
                .OrderBy(r => r.Type)
                .ToListAsync();
        }

        public async Task<Rekening> MaakRekeningAanAsync(int gebruikerId, RekeningType type)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(gebruikerId);
            if (gebruiker == null)
                throw new ArgumentException("Gebruiker niet gevonden");

            string iban = GenereerIBAN();

            while (await _context.Rekeningen.AnyAsync(r => r.Iban == iban))
            {
                iban = GenereerIBAN();
            }

            var rekening = new Rekening
            {
                GebruikerId = gebruikerId,
                Iban = iban,
                Type = type,
                Saldo = 0
            };

            _context.Rekeningen.Add(rekening);
            await _context.SaveChangesAsync();

            return rekening;
        }

        public async Task<bool> UpdateSaldoAsync(int rekeningId, decimal nieuwSaldo)
        {
            var rekening = await _context.Rekeningen.FindAsync(rekeningId);
            if (rekening == null)
                return false;

            rekening.Saldo = nieuwSaldo;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotaalSaldoAsync(int gebruikerId)
        {
            return await _context.Rekeningen
                .Where(r => r.GebruikerId == gebruikerId)
                .SumAsync(r => r.Saldo);
        }

        private string GenereerIBAN()
        {
            Random random = new Random();
            string accountNummer = random.Next(100000000, 999999999).ToString().PadLeft(12, '0');
            int checkDigit = random.Next(10, 99);
            return $"BE{checkDigit}{accountNummer}";
        }
    }
}