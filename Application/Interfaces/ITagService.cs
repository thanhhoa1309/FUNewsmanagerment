using Repository.DTOs.Tag;

namespace Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagResponseDTO>> GetAllTagsAsync();
        Task<TagResponseDTO?> GetTagByIdAsync(int tagId);
        Task<(bool Success, string Message)> CreateTagAsync(TagCreateDTO tagDto);
        Task<(bool Success, string Message)> UpdateTagAsync(int tagId, TagUpdateDTO tagDto);
        Task<(bool Success, string Message)> DeleteTagAsync(int tagId);
        Task<List<TagResponseDTO>> SearchTagsAsync(string searchTerm);
    }
}
