﻿using Chirp.Core;
using Chirp.Core.DTOs;
using Chirp.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using CheepDTO = Chirp.Core.DTOs.CheepDTO;

namespace Chirp.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly CheepDBContext _dbContext;

        public AuthorRepository(CheepDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<AuthorDTO?> FindAuthorByNameDTO(string name)
        {
            var author = await (from a in _dbContext.Authors
                where a.Name == name
                select new AuthorDTO
                {
                    AuthorId = a.AuthorId,
                    Name = a.Name,
                    Email = a.Email,
                    AuthorsFollowed = a.AuthorsFollowed
                }).FirstOrDefaultAsync();

            return author;
        }
        
        // Find The author by name
        public async Task<Author?> FindAuthorByName(string name)
        {
            return await _dbContext.Authors
                .FirstOrDefaultAsync(a => a.Name == name);
        }
        
        // Used for creating a new author when the author is not existing
        public async Task CreateAuthor(string authorName, string authorEmail, string profilePicture)
        {
            string? base64ProfilePicture = null;
            if (profilePicture != null)
            {
                base64ProfilePicture = await DownloadAndConvertToBase64Async(profilePicture);
            }
            
            var author = new Author()
            {   
                Name = authorName,
                Email = authorEmail,
                Cheeps = new List<Cheep>(),
                AuthorsFollowed = new List<string>(),
                ProfilePicture = base64ProfilePicture
            };
            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync(); // Persist the changes to the database
        }
        
        public async Task DeleteUser(AuthorDTO Author)
        {
            // Retrieve the author by name and then remove them
            var author = await _dbContext.Authors
                .FirstOrDefaultAsync(a => a.Name == Author.Name);

            if (author != null)
            {
                _dbContext.Authors.Remove(author);
            }

            await _dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Follows an author
        /// </summary>
        /// <param name="userAuthorName"></param>
        /// <param name="followedAuthorName"></param>
        public async Task FollowAuthor(string userAuthorName, string followedAuthorName)
        {
            var author = await FindAuthorByName(userAuthorName); // Find the author by name
            if (author != null && !author.AuthorsFollowed.Contains(followedAuthorName)) // Check if the author is not already followed
            {
                author.AuthorsFollowed.Add(followedAuthorName); // Add the author to the list of followed authors
                await _dbContext.SaveChangesAsync(); // Persist the changes to the database
            }
        }

        /// <summary>
        /// Unfollows an author
        /// </summary>
        /// <param name="userAuthorName"></param>
        /// <param name="authorToBeRemoved"></param>
        public async Task UnfollowAuthor(string userAuthorName, string authorToBeRemoved)
        {
            var author = await FindAuthorByName(userAuthorName); // Find the author by name
            if (author != null && author.AuthorsFollowed.Contains(authorToBeRemoved)) // Check if the author is followed
            {
                author.AuthorsFollowed.Remove(authorToBeRemoved); // Remove the author from the list of followed authors
                await _dbContext.SaveChangesAsync(); // Persist the changes to the database
            }
        }

        /// <summary>
        /// When deleting user data we need to delete the username for every other author list. 
        /// </summary>
        /// <param name="authorName"></param>
        /// <param name="page"></param>
        public async Task RemovedAuthorFromFollowingList(string authorName)
        {
            // Fetch all authors that follows a specific author 
            var authors = await _dbContext.Authors
                .Where(author => author.AuthorsFollowed.Contains(authorName))
                .ToListAsync();
            
            // Remove the specific author from the list for all authors
            foreach (var author in authors) 
            {
                author.AuthorsFollowed.Remove(authorName);
            }
            
            await _dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Returns the list of authors that a user follows
        /// </summary>
        /// <param name="userName">The username from the url</param>
        /// <returns></returns>
        public async Task<List<string>> GetFollowedAuthors(string userName)
        {
            var author = await Task.Run(() => FindAuthorByName(userName));
            return author?.AuthorsFollowed.ToList() ?? new List<string>();
        }
        
        /// <summary>
        /// Returns the list of authors that follows a user
        /// </summary>
        /// <param name="userName">The username from the url</param>
        /// <returns></returns>
        public async Task<List<string>> GetFollowingAuthors(string userName)
        {
            var author = await Task.Run(() => FindAuthorByName(userName));
            var followingAuthors = new List<string>();
            foreach (Author a in _dbContext.Authors) 
            {
                if (a.AuthorsFollowed.Contains(userName))
                {
                    followingAuthors.Add(a.Name);
                }
            }

            return followingAuthors;
        }

        public async Task<int> GetKarmaForAuthor(string authorName)
        {
            var author = await FindAuthorByName(authorName);

            // Check for if author is null
            if (author == null) return 0;
            
            var cheepIds = await _dbContext.Cheeps
                .Where(cheep => cheep.AuthorId == author.AuthorId)
                .Select(cheep => cheep.CheepId)
                .ToListAsync();

            var likesCount = await _dbContext.Likes
                .CountAsync(like => cheepIds.Contains(like.CheepId));

            var dislikesCount = await _dbContext.Dislikes
                .CountAsync(dislike => cheepIds.Contains(dislike.CheepId));

            var karma = likesCount - dislikesCount;
            return karma;
        }
        
        public async Task<string> DownloadAndConvertToBase64Async(string imageUrl)
        {
            // Create an HTTP client
            using var httpClient = new HttpClient();

            try
            {
                // Download the image as a byte array
                byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                // Convert the byte array to a Base64 string
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or converting image: {ex.Message}");
                throw;
            }
        }
    }    
}
