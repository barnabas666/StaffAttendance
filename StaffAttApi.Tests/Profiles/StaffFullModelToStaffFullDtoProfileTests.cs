using AutoMapper;
using FluentAssertions;
using StaffAttApi.Profiles;
using StaffAttLibrary.Models;
using StaffAttShared.DTOs;

namespace StaffAttApi.Tests.Profiles;
public class StaffFullModelToStaffFullDtoProfileTests
{
    [Fact]
    public void TestStaffFullMapping()
    {
        // Arrange
        var config = new MapperConfiguration(cfg =>
        {
            // Register the profile under test
            cfg.AddProfile<StaffFullModelToStaffFullDtoProfile>();
            // Register dependent profiles used for nested objects / collections            
            cfg.AddProfile<AddressModelToAddressDtoProfile>();
            cfg.AddProfile<PhoneNumberModelToPhoneNumberDtoProfile>();
        });

        var mapper = config.CreateMapper();

        var model = new StaffFullModel
        {
            Id = 10,
            FirstName = "Bob",
            LastName = "Brown",
            EmailAddress = "bob.brown@example.com",
            Alias = "BB",
            IsApproved = false,
            DepartmentId = 3,
            Title = "Analyst",
            AddressId = 4,
            AliasId = 9,
            Description = "Finance department staff",
            Address = new AddressModel { Id = 1, Street = "Elm St", City = "Springfield", Zip = "12345", State = "IL" },
            PhoneNumbers = new List<PhoneNumberModel>
            {
                new PhoneNumberModel { Id = 1, PhoneNumber = "123456789" },
                new PhoneNumberModel { Id = 2, PhoneNumber = "987654321" }
            }
        };

        // Act
        var dto = mapper.Map<StaffFullDto>(model);

        // Assert top-level members
        dto.Id.Should().Be(model.Id);
        dto.FirstName.Should().Be(model.FirstName);
        dto.LastName.Should().Be(model.LastName);
        dto.EmailAddress.Should().Be(model.EmailAddress);
        dto.Alias.Should().Be(model.Alias);
        dto.IsApproved.Should().Be(model.IsApproved);
        dto.DepartmentId.Should().Be(model.DepartmentId);
        dto.Title.Should().Be(model.Title);
        dto.AddressId.Should().Be(model.AddressId);
        dto.AliasId.Should().Be(model.AliasId);
        dto.Description.Should().Be(model.Description);

        // Nested address
        dto.Address.Should().NotBeNull();
        dto.Address.Street.Should().Be(model.Address.Street);
        dto.Address.City.Should().Be(model.Address.City);
        dto.Address.Zip.Should().Be(model.Address.Zip);
        dto.Address.State.Should().Be(model.Address.State);

        // Phone numbers mapping: ensure count and values
        dto.PhoneNumbers.Should().HaveCount(model.PhoneNumbers.Count);
        dto.PhoneNumbers.Select(p => p.PhoneNumber).Should().BeEquivalentTo(model.PhoneNumbers.Select(p => p.PhoneNumber));
    }
}
