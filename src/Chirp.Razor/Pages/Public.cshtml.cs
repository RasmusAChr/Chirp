﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly CheepRepository _service;
    public int pageNumber { get; set; }
    public List<CheepDTO> Cheeps { get; set; }

    public PublicModel(CheepRepository service)
    {
        _service = service;
    }
    /// <summary>
    /// Gets cheeps and stores them in a list, when the page is loaded
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnGet([FromQuery] int page)
    {
        if (!(page is int) || page <= 0)
        {
            page = 1;
        }
        
        pageNumber = page;
        Cheeps = await _service.ReadAllCheeps(page);
        return Page();
    }
}