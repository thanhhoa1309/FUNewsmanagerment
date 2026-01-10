using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Tag;
using Repository.Entities;
using Repository.Interfaces;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TagResponseDTO>> GetAllTagsAsync()
        {
            try
            {
                var tags = await _unitOfWork.TagRepository
                    .GetAllAsQueryable()
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();

                return tags.Select(t => new TagResponseDTO
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<TagResponseDTO>();
            }
        }

        public async Task<TagResponseDTO?> GetTagByIdAsync(int tagId)
        {
            try
            {
                var tag = await _unitOfWork.TagRepository.GetByIdAsync(tagId);

                if (tag == null || tag.IsDeleted)
                {
                    return null;
                }

                return new TagResponseDTO
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Note = tag.Note,
                    CreatedAt = tag.CreatedAt,
                    UpdatedAt = tag.UpdatedAt
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateTagAsync(TagCreateDTO tagDto)
        {
            try
            {
                var existingTag = await _unitOfWork.TagRepository.FirstOrDefaultAsync(
                    t => t.TagName == tagDto.TagName && !t.IsDeleted
                );

                if (existingTag != null)
                {
                    return (false, "Tag name already exists.");
                }

                var tag = new Tag
                {
                    TagName = tagDto.TagName,
                    Note = tagDto.Note,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var created = await _unitOfWork.TagRepository.CreateAsync(tag);
                if (created)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Tag created successfully.");
                }

                return (false, "Failed to create tag.");
            }
            catch (Exception ex)
            {
                return (false, $"Create failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateTagAsync(int tagId, TagUpdateDTO tagDto)
        {
            try
            {
                var tag = await _unitOfWork.TagRepository.GetByIdAsync(tagId);

                if (tag == null || tag.IsDeleted)
                {
                    return (false, "Tag not found.");
                }

                if (!string.IsNullOrEmpty(tagDto.TagName))
                {
                    var existingTag = await _unitOfWork.TagRepository.FirstOrDefaultAsync(
                        t => t.TagName == tagDto.TagName && t.TagId != tagId && !t.IsDeleted
                    );

                    if (existingTag != null)
                    {
                        return (false, "Tag name already exists.");
                    }

                    tag.TagName = tagDto.TagName;
                }

                if (tagDto.Note != null)
                    tag.Note = tagDto.Note;

                tag.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.TagRepository.UpdateAsync(tag);
                if (updated)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Tag updated successfully.");
                }

                return (false, "Failed to update tag.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteTagAsync(int tagId)
        {
            try
            {
                var tag = await _unitOfWork.TagRepository.GetByIdAsync(tagId);

                if (tag == null || tag.IsDeleted)
                {
                    return (false, "Tag not found.");
                }

                tag.IsDeleted = true;
                tag.DeletedAt = DateTime.UtcNow;

                var deleted = await _unitOfWork.TagRepository.UpdateAsync(tag);
                if (deleted)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Tag deleted successfully.");
                }

                return (false, "Failed to delete tag.");
            }
            catch (Exception ex)
            {
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<List<TagResponseDTO>> SearchTagsAsync(string searchTerm)
        {
            try
            {
                var tags = await _unitOfWork.TagRepository
                    .GetAllAsQueryable()
                    .Where(t => !t.IsDeleted && 
                           (t.TagName.Contains(searchTerm) || 
                            (t.Note != null && t.Note.Contains(searchTerm))))
                    .ToListAsync();

                return tags.Select(t => new TagResponseDTO
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<TagResponseDTO>();
            }
        }
    }
}
