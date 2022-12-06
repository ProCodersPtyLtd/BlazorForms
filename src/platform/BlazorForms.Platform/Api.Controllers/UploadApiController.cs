using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorForms.Rendering.RadzenBlazor.Controllers;

[ApiController]
[Route("_api/pc/BlazorForms/radzen/fileupload")]
public class UploadApiController : ControllerBase
{
    private static ConcurrentDictionary<string, byte[]> _files = new(); // TODO YB: Use InMemory cache

    [HttpGet]
    public string Index()
    {
        return "Hello, World!";
    }

    [HttpPost("{fileId}")]
    public async Task<IActionResult> PostFile(string fileId, IFormFile file)
    {
        try
        {
            if (file.Length > int.MaxValue)
                throw new ApplicationException($"Cannot upload files larger then {int.MaxValue} bytes");

            using var fileMemo = new MemoryStream((int) file.Length);
            await file.CopyToAsync(fileMemo);
            var fileData = fileMemo.ToArray();
            _files.AddOrUpdate(fileId, _ => fileData, (_, _) => fileData);
            return Ok(new {Completed = true});
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{fileId}")]
    public async Task<byte[]> GetFile(string fileId)
    {
        return _files.TryGetValue(fileId, out var result) ? result : null;
    }
    
    [HttpDelete("{fileId}")]
    public async Task DeleteFile(string fileId)
    {
        _files.TryRemove(fileId, out _);
    }
}