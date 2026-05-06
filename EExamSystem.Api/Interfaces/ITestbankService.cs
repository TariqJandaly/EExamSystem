using EExamSystem.Shared.DTOs.Testbanks;
using EExamSystem.Shared.DTOs;
namespace EExamSystem.Api.Interfaces;


/// <summary>
/// Interface for managing testbanks, which are collections of exam questions associated with specific courses.
/// </summary>
public interface ITestbankService
{
    Task<ServiceResponse<List<TestbankDto>>> GetAllTestbanksAsync();
    Task<ServiceResponse<TestbankDto>> GetTestbankAsync(int id);
    Task<ServiceResponse<TestbankDto>> CreateTestbankAsync(TestbankCreateDto testbankCreateDto);
    Task<ServiceResponse<TestbankDto>> DeleteTestbankAsync(int id);
    Task<ServiceResponse<TestbankDto>> UpdateTestbankAsync(int id, TestbankCreateDto testbankCreateDto);

}