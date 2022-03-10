using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

public class SocialCardModel : PageModel
{
    [BindProperty(Name = "title", SupportsGet = true)]
    public string Title { get; set; }

    [BindProperty(Name = "desc", SupportsGet = true)]
    public string Description { get; set; }

    [BindProperty(Name = "tags", SupportsGet = true)]
    public string Tags { get; set; }

    public void OnGet()
    {
    }
}
