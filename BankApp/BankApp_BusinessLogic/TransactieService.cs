using BankApp_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApp_BusinessLogic
{
    public class TransactieService : ITransactieService
    {
        private readonly AppDbContext _context;

        public TransactieService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transactie> GetTransactieByIdAsync(int transactieId)
        {
            return await _context.Transacties
                .Include(t => t.Gebruiker)
                .FirstOrDefaultAsync(t => t.Id == transactieId);
        }

        public async Task<List<Transactie>> GetTransactiesByRekeningIdAsync(int rekeningId, int aantal = 50)
        {
            var rekening = await _context.Rekeningen.FindAsync(rekeningId);
            if (rekening == null)
                return new List<Transactie>();

            return await _context.Transacties
                .Where(t => t.VanIban == rekening.Iban || t.NaarIban == rekening.Iban)
                .OrderByDescending(t => t.Datum)
                .Take(aantal)
                .ToListAsync();
        }

        public async Task<List<Transactie>> GetTransactiesByGebruikerIdAsync(int gebruikerId, int aantal = 50)
        {
            var gebruikerIbans = await _context.Rekeningen
                .Where(r => r.GebruikerId == gebruikerId)
                .Select(r => r.Iban)
                .ToListAsync();

            return await _context.Transacties
                .Where(t => gebruikerIbans.Contains(t.VanIban) || gebruikerIbans.Contains(t.NaarIban))
                .OrderByDescending(t => t.Datum)
                .Take(aantal)
                .ToListAsync();
        }

        public async Task<(bool Succes, string Bericht, Transactie Transactie)> MaakOverschrijvingAsync(
            string vanIban,
            string naarIban,
            decimal bedrag,
            string omschrijving,
            int gebruikerId)
        {
            if (bedrag <= 0)
                return (false, "Bedrag moet groter zijn dan 0", null);

            var vanRekening = await _context.Rekeningen
                .FirstOrDefaultAsync(r => r.Iban == vanIban);

            if (vanRekening == null)
                return (false, "Bronrekening niet gevonden", null);

            if (vanRekening.GebruikerId != gebruikerId)
                return (false, "U bent niet gemachtigd voor deze rekening", null);

            if (vanRekening.Saldo < bedrag)
                return (false, "Onvoldoende saldo", null);

            var naarRekening = await _context.Rekeningen
                .FirstOrDefaultAsync(r => r.Iban == naarIban);

            if (naarRekening == null)
                return (false, "Doelrekening niet gevonden", null);

            if (vanIban == naarIban)
                return (false, "Kan niet naar dezelfde rekening overschrijven", null);

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                vanRekening.Saldo -= bedrag;
                naarRekening.Saldo += bedrag;

                var transactie = new Transactie
                {
                    VanIban = vanIban,
                    NaarIban = naarIban,
                    NaamOntvanger = naarRekening.Gebruiker?.Email ?? "Onbekend",
                    Bedrag = bedrag,
                    Omschrijving = omschrijving,
                    Datum = DateTime.Now,
                    GebruikerId = gebruikerId
                };

                _context.Transacties.Add(transactie);
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return (true, "Transactie succesvol uitgevoerd", transactie);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return (false, $"Transactie mislukt: {ex.Message}", null);
            }
        }
    }
}