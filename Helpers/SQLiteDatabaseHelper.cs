using Rba.Models;
using SQLite;

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

        // criação da tabela ponto de coleta
        _database.CreateTableAsync<PontoColeta>()
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
        return _database.Table<Usuario>()
            .Where(u => u.NomeUsuario == nomePadrao)
            .FirstOrDefaultAsync();
    }
    public Task<List<Usuario>> GetUsuariosAsync() =>
        _database.Table<Usuario>()
        .ToListAsync();
    
    // Tipo de lixo
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
    public Task<int> DeleteTipoLixo(int Id)
    {
        return _database.Table<TipoLixo>().DeleteAsync(i => i.ID == Id);
    }
    public Task<int> UpdateTl(TipoLixo tipoLixo)
    {
        string sql = "UPDATE TipoLixo SET Cor=?,Material = ?, Origem = ?,  OrigemDescricao = ?,  DestinoAmbiental = ?, Exemplos = ? WHERE ID = ? ";
        return _database.ExecuteAsync(sql, tipoLixo.Cor, tipoLixo.Material, tipoLixo.Origem, tipoLixo.OrigemDescricao, tipoLixo.DestinoAmbiental, tipoLixo.Exemplos, tipoLixo.ID);
    }

    // POntos de coletas 
    public Task<List<PontoColeta>> GetAll()
    {
        return _database.Table<PontoColeta>().ToListAsync();
    }

    public Task<int> Update(PontoColeta p)
    {
        string sql = "UPDATE PontoColeta SET Nome=?, Endereco=?, TipoLixo=?, Contato=?, Horario=? WHERE Id=?";
        return _database.ExecuteAsync(sql, p.Nome, p.Endereco, p.TipoLixo, p.Contato, p.Horario, p.Id);
    }

    public Task<int> Delete(int Id)
    {
        return _database.Table<PontoColeta>().DeleteAsync(i => i.Id == Id);
    }

    public Task<int> Insert(PontoColeta pc)
    {
        return _database.InsertAsync(pc);
    }

    // Usuário
    public Task<int> DeleteUsuarioAsync(int id)
    {
        return _database.Table<Usuario>().DeleteAsync(u => u.ID == id);
    }
    public static class Sessao
    {
        public static Usuario UsuarioPage { get; private set; }

        public static void IniciarSessao(Usuario usuario)
        {
            UsuarioPage = usuario;
        }
        public static void EncerrarSessao()
        {
            UsuarioPage = null;
        }
        public static bool IsMaster => UsuarioPage != null && UsuarioPage.TipoUsuario == "Master";

        
    }
}
