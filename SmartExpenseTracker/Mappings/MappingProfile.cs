using AutoMapper;
using SmartExpenseTracker.CQRS.Commands;
using SmartExpenseTracker.DTOs;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Expense, ExpenseDto>();
            CreateMap<CreateExpenseDto, AddExpenseCommand>();
            CreateMap<AddExpenseCommand, Expense>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}
