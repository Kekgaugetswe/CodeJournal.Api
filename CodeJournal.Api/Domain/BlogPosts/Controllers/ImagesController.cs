using CodeJournal.Api.Domain.BlogPosts.Dtos;
using CodeJournal.Api.Domain.BlogPosts.Models;
using CodeJournal.Api.Domain.BlogPosts.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CodeJournal.Api.Domain.BlogPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        public IImageRepository imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }


        //POST: {apibaseurl}/api/images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageModel uploadImageModel )
        {
            ValidateFileUpload(uploadImageModel.UploadImage);
            if (ModelState.IsValid)
            {
                var blogImage = new BlogImage
                {
                    FileExtension = Path.GetExtension(uploadImageModel.UploadImage.FileName).ToLower(),
                    FileName = uploadImageModel.FileName,
                    Title = uploadImageModel.Title,
                    DateCreated = DateTime.Now
                };


                blogImage = await imageRepository.Upload(uploadImageModel.UploadImage, blogImage);

                // convert domain Model to DTO

                var response = new BlogImageDto
                {
                    Id = blogImage.Id,
                    FileName = blogImage.FileName,
                    FileExtension = blogImage.FileExtension,
                    Title = blogImage.Title,
                    Url = blogImage.Url,
                    DateCreated = blogImage.DateCreated
                };

                return Ok(response);
            }
            return BadRequest(ModelState);


        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file format");
            }
            if (file.Length > 10405760)
            {
                ModelState.AddModelError("file", "File size exceeds 10MB limit");

            }


        }
    }
}
