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
        // Cria as tabelas se não existirem

        // criação da tabela usuário
        _database.CreateTableAsync<Usuario>()
            .ConfigureAwait(false)
            .GetAwaiter().GetResult();

        // criação da tabela tipo de lixo
        _database.CreateTableAsync<TipoLixo>()
            .ConfigureAwait(false)
            .GetAwaiter().GetResult();
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

    public Task<int> InserirTipoLixoAsync(TipoLixo tipoLixo)
    {
        tipoLixo.Cor = tipoLixo.Cor.Trim();
        tipoLixo.Material = tipoLixo.Material.Trim();
        tipoLixo.Origem = tipoLixo.Origem.Trim();
        tipoLixo.OrigemDescricao = tipoLixo.OrigemDescricao.Trim();
        tipoLixo.DestinoAmbiental = tipoLixo.DestinoAmbiental.Trim();
        tipoLixo.Exemplos = tipoLixo.Exemplos.Trim();

        return _database.InsertAsync(tipoLixo);
    }

    public Task<List<TipoLixo>> GetTiposLixosAsync() =>
        _database.Table<TipoLixo>()
        .ToListAsync();
}
