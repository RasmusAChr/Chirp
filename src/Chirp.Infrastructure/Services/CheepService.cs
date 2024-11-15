using Chirp.Core;
using Chirp.Core.DTOs;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;
public interface ICheepService
{
    public Task<List<Core.DTOs.CheepDTO>> GetCheeps(int page);
    public Task<List<Core.DTOs.CheepDTO>> GetCheepsFromAuthor(string author, int page);
    
    public Task<int> GetTotalPageNumber(string authorName);
    
    public Task<List<Core.DTOs.CheepDTO>> RetrieveAllCheeps();
    
    public Task CreateCheep(CheepDTO Cheep);
    
    public Task DeleteCheepsByAuthor(AuthorDTO Author);
}
public class CheepService : ICheepService
{
    private readonly CheepRepository _cheepRepository;
    public CheepService(CheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<int> GetTotalPageNumber(string authorName = "")
    {
        return await _cheepRepository.GetTotalPages(authorName);
    }
    public async Task<List<Core.DTOs.CheepDTO>> GetCheeps(int page)
    {
        return await _cheepRepository.ReadAllCheeps(page);
    }
    
    public async Task<List<Core.DTOs.CheepDTO>> GetCheepsFromAuthor(string author, int page)
    {
        return await _cheepRepository.ReadCheepsFromAuthor(author, page);
    }
    
    public async Task<List<Core.DTOs.CheepDTO>> RetrieveAllCheeps()
    {
        return await _cheepRepository.RetrieveAllCheepsForEndPoint();
    }

    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        await _cheepRepository.CreateAuthor(authorName, authorEmail);
    }

    public async Task<Author>? FindAuthorByName(String name)
    {
        return _cheepRepository.FindAuthorByName(name);
    }
    
    public async Task CreateCheep(CheepDTO Cheep)
    {
        await _cheepRepository.CreateCheep(Cheep);
    }
    public async Task DeleteCheepsByAuthor(AuthorDTO Author)
    {
        await _cheepRepository.DeleteCheepsByAuthor(Author);
    }
    public async Task DeleteCheep(int cheepId)
    {
        await _cheepRepository.DeleteCheep(cheepId);
    }

    public async Task FollowAuthor(string userAuthorName, string followedAuthorName)
    {
        await _cheepRepository.FollowAuthor(userAuthorName, followedAuthorName);
    }

    public async Task UnfollowAuthor(string userAuthor, string authorToBeRemoved)
    {
        await _cheepRepository.UnfollowAuthor(userAuthor, authorToBeRemoved);
    }
}