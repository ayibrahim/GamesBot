using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Servers
    {
        private readonly MainContext _context;

        public Servers(MainContext context) {
            _context = context;
        }

        public async Task AddNewGuild(long id , string prefix) {
           _context.Add(new Server { Id = id, Prefix = prefix , Lang = "ar"});
           await _context.SaveChangesAsync();
        }

        public async Task RemoveGuild(long id) {
            var server = await _context.Servers.FindAsync(id);
            _context.Remove(server);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyGuildPrefix(long id, string prefix) 
        {
            var server = await _context.Servers.FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id, Prefix = prefix });
            }
            else {
                server.Prefix = prefix;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGuildPrefix(long id) {
            var prefix = await _context.Servers.Where(x => x.Id == id).Select(x => x.Prefix).FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }


        public async Task ModifyGuildLang(long id, string lang)
        {
            var server = await _context.Servers.FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id, Lang = lang });
            }
            else
            {
                server.Lang = lang;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGuildLang(long id)
        {
            var lang = await _context.Servers.Where(x => x.Id == id).Select(x => x.Lang).FirstOrDefaultAsync();
            return await Task.FromResult(lang);
        }
    }
}
