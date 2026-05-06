using EExamSystem.Api.Data;
using EExamSystem.Api.Interfaces;
using EExamSystem.Shared.DTOs.Testbanks;
using EExamSystem.Shared.DTOs;
using EExamSystem.Shared.Models;

public class TestbankService : ITestbankService
{

    private readonly AppDbContext _context;

    public TestbankService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<TestbankDto>> CreateTestbankAsync(TestbankCreateDto testbankCreateDto)
    {
        var testbank = new Testbank
        {
            Name = testbankCreateDto.Name,
            CourseId = testbankCreateDto.CourseId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Testbanks.Add(testbank);
        await _context.SaveChangesAsync();

        return new ServiceResponse<TestbankDto>
        {
            Success = true,
            Data = new TestbankDto
            {
                Id = testbank.Id,
                Name = testbank.Name,
                CreatedAt = testbank.CreatedAt,
                CourseId = testbank.CourseId
            },
            StatusCode = 201
        };
    }

    public async Task<ServiceResponse<TestbankDto>> DeleteTestbankAsync(int id)
    {
        var testbank = await _context.Testbanks.FindAsync(id);
        if (testbank == null)
        {
            return new ServiceResponse<TestbankDto>
            {
                Success = false,
                Message = "TestbankNotFound",
                StatusCode = 404
            };
        }

        _context.Testbanks.Remove(testbank);
        await _context.SaveChangesAsync();

        return new ServiceResponse<TestbankDto>
        {
            Success = true,
            Data = new TestbankDto
            {
                Id = testbank.Id,
                Name = testbank.Name,
                CreatedAt = testbank.CreatedAt,
                CourseId = testbank.CourseId
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<TestbankDto>> GetTestbankAsync(int id)
    {
        var testbank = await _context.Testbanks.FindAsync(id);

        if (testbank == null)
        {
            return new ServiceResponse<TestbankDto>
            {
                Success = false,
                Message = "TestbankNotFound",
                StatusCode = 404
            };
        }

        return new ServiceResponse<TestbankDto>
        {
            Success = true,
            Data = new TestbankDto
            {
                Id = testbank.Id,
                Name = testbank.Name,
                CreatedAt = testbank.CreatedAt,
                CourseId = testbank.CourseId
            },
            StatusCode = 200
        };
    }

    public async Task<ServiceResponse<TestbankDto>> UpdateTestbankAsync(int id, TestbankCreateDto testbankCreateDto)
    {
        var testbank = await _context.Testbanks.FindAsync(id);
        if (testbank == null)
        {
            return new ServiceResponse<TestbankDto>
            {
                Success = false,
                Message = "TestbankNotFound",
                StatusCode = 404
            };
        }

        testbank.Name = testbankCreateDto.Name;
        testbank.CourseId = testbankCreateDto.CourseId;

        _context.Testbanks.Update(testbank);
        await _context.SaveChangesAsync();

        return new ServiceResponse<TestbankDto>
        {
            Success = true,
            Data = new TestbankDto
            {
                Id = testbank.Id,
                Name = testbank.Name,
                CreatedAt = testbank.CreatedAt,
                CourseId = testbank.CourseId
            },
            StatusCode = 200
        };
    }
}