using SQLite;
using Rba.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Rba.Helpers;

public class SQLiteDatabaseHelper
{
    private readonly SQLiteAsyncConnection _database;

    public SQLiteDatabaseHelper(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Usuario>().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public Task<int> InserirUsuarioAsync(Usuario usuario)
    {
        usuario.NomeUsuario = usuario.NomeUsuario.Trim();
        usuario.Email = usuario.Email.Trim();
        usuario.Senha = usuario.Senha.Trim();
        usuario.TipoUsuario = usuario.TipoUsuario.Trim();

        return _database.InsertAsync(usuario);
    }

    public Task<Usuario?> ObterUsuarioPorNomeAsync(string nome)
    {
        string nomePadrao = nome.Trim();
        return _database.Table<Usuario?>()
            .Where(u => u.NomeUsuario == nomePadrao)
            .FirstOrDefaultAsync();
    }
    public Task<List<Usuario>> GetUsuariosAsync() =>
        _database.Table<Usuario>()
        .ToListAsync();
}
