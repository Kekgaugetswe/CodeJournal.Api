using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApplicationDbContext dbContext;

    public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<BlogImage>> GetAll()
    {
       return await dbContext.BlogImages.ToListAsync();
    }

    public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
    {

        // 1- upload image top api/images
        var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "images", $"{blogImage.FileName}{blogImage.FileExtension}");

        using var stream = new FileStream(localPath, FileMode.Create);

        await file.CopyToAsync(stream);

        // 2- update the database with the image url
        // example:  https://codejournal.com/images/somefilename.jpg
        var httpRequest = httpContextAccessor.HttpContext.Request;
        var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}/images/{blogImage.FileName}{blogImage.FileExtension}";
        blogImage.Url = urlPath;

        await dbContext.BlogImages.AddAsync(blogImage);
        await dbContext.SaveChangesAsync();

        return blogImage;


    }
}
