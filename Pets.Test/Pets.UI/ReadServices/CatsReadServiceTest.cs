using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Pets.Application.People;
using Pets.UI.ReadServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pets.Test.Pets.UI.ReadServices
{
    public class Tests
    {
        //TODO Is it the responsibility of our system to check validity of data e.g. if gender not specified
        //or pet name not specified?  If they aren't, should it throw an exception, or just log it, or just gracefully
        //ignore it?

        //TODO Should our DTO's have validation attributes e.g. age must be number greater than 0 etc.

        //TODO I've used Moq to mock the IPeopleService BUT we aren't testing the mock, we just need it to return data so we can
        //test the read service... so should I be using something other than Moq?  i.e. usually when using Moq you would
        //call Verify to check if the method on the mock was called once or something like that, but I don't care, I just need
        //it to return data.

        Mock<IPeopleService> _mockPeopleService;
        PersonDto _maleOwner1; //TODO What do you normally use as the naming convention, underscore, captial, other? Different location for this data like some sort of "fixture"(?)
        PersonDto _maleOwner2;
        PersonDto _femaleOwner1;
        PersonDto _femaleOwner2;
        PersonDto _transgenderOwner1;
        PetDto _cat1;
        PetDto _cat2;
        PetDto _cat3;
        PetDto _cat4;
        PetDto _cat5;
        PetDto _cat6;
        PetDto _dog1;
        PetDto _dog2;
        PetDto _dog3;
        IEnumerable<PersonDto> _owners;

        [SetUp]
        public void Setup()
        {
            //TODO Is there a better / cleaner / nicer way to setup all the required test data?
            _mockPeopleService = new Mock<IPeopleService>(MockBehavior.Strict);

            _maleOwner1 = new PersonDto() { Gender = "Male", Age = 21, Name = "MaleOwner1" }; //TODO Nuget package to generate random strings/ints?
            _maleOwner2 = new PersonDto() { Gender = "Male", Age = 22, Name = "MaleOwner2" };
            _femaleOwner1 = new PersonDto() { Gender = "Female", Age = 31, Name = "FemaleOwner1" };
            _femaleOwner2 = new PersonDto() { Gender = "Female", Age = 32, Name = "FemaleOwner2" };
            _transgenderOwner1 = new PersonDto() { Gender = "Transgender", Age = 41, Name = "TransgenderOwner1" };
            _cat1 = new PetDto() { Name = "Cat1", Type = "Cat" };
            _cat2 = new PetDto() { Name = "Cat2", Type = "Cat" };
            _cat3 = new PetDto() { Name = "Cat3", Type = "Cat" };
            _cat4 = new PetDto() { Name = "Cat4", Type = "Cat" };
            _cat5 = new PetDto() { Name = "Cat5", Type = "Cat" };
            _cat6 = new PetDto() { Name = "Cat6", Type = "Cat" };
            _dog1 = new PetDto() { Name = "Dog1", Type = "Dog" };
            _dog2 = new PetDto() { Name = "Dog2", Type = "Dog" };
            _dog3 = new PetDto() { Name = "Dog3", Type = "Dog" };
        }

        [Test]
        public async Task Returns_GendersRelatedToPetCatOwnersOnly()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat1 };
            _femaleOwner1.Pets = new List<PetDto> { _dog2 };
            _owners = new List<PersonDto> { _maleOwner1, _femaleOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 1);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].OwnerGender, _maleOwner1.Gender);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 1);
        }
         
        [Test]
        public async Task Returns_MoreGendersThanMaleAndFemaleIfOwnerIdentifiedAsAnotherGender()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat1 };
            _femaleOwner1.Pets = new List<PetDto> { _cat2 };
            _transgenderOwner1.Pets = new List<PetDto> { _cat3 };
            _owners = new List<PersonDto> { _maleOwner1, _femaleOwner1, _transgenderOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 3);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList().Select(g => g.OwnerGender).Contains(_maleOwner1.Gender), true);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList().Select(g => g.OwnerGender).Contains(_femaleOwner1.Gender), true);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList().Select(g => g.OwnerGender).Contains(_transgenderOwner1.Gender), true);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 1);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 1);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 1);
            //TODO I've just used AreEqual everywhere... which does what I need... just wondering if I should be using
            //different terms for perhaps clarity or perhaps something like Shouldy or Fluent Assertions? (I believe some
            //people think they make it more readable/understandable).
        }

        [Test]
        public async Task Returns_CatEvenIfOwnerAlsoOwnsADog()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat1, _dog1 };
            _owners = new List<PersonDto> { _maleOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 1);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 1);
        }

        [Test]
        public async Task Returns_MoreThanOneCatNameIfAOwnerHasMoreThanOneCat()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat1, _cat2, _cat3 };
            _owners = new List<PersonDto> { _maleOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.Count(), 3);
        }

        [Test]
        public async Task Returns_WithoutThrowingExceptionWhenOnePersonHasNoPets()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat1 };
            _owners = new List<PersonDto> { _maleOwner1, _femaleOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 1);
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList().Select(g => g.OwnerGender).Contains(_maleOwner1.Gender), true);
        }

        [Test]
        public async Task Returns_GendersAndPetNamesInAlphabeticOrder()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _cat6 };
            _maleOwner2.Pets = new List<PetDto> { _cat3 };
            _femaleOwner1.Pets = new List<PetDto> { _cat1, _cat4 };
            _femaleOwner2.Pets = new List<PetDto> { _cat5 };
            _transgenderOwner1.Pets = new List<PetDto> { _cat2 };
            _owners = new List<PersonDto> { _maleOwner1, _maleOwner2, _transgenderOwner1, _femaleOwner1, _femaleOwner2};

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            //...Genders are sorted alphabetically
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].OwnerGender, "Female");
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[1].OwnerGender, "Male");
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[2].OwnerGender, "Transgender");

            //...Cat names for Female owners sorted alphabetically
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.ToList()[0], "Cat1");
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.ToList()[1], "Cat4");
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[0].CatNames.ToList()[2], "Cat5");

            //...Cat names for Male owners sorted alphabetically
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[1].CatNames.ToList()[0], "Cat3");
            Assert.AreEqual(catOwnersViewModel.CatsByGender.ToList()[1].CatNames.ToList()[1], "Cat6");
        }

        [Test]
        public async Task Returns_EmptyModelIfThereAreOwnersButNoneOwnCats()
        {
            //Arrange
            _maleOwner1.Pets = new List<PetDto> { _dog1 };
            _maleOwner1.Pets = new List<PetDto> { _dog2 };
            _owners = new List<PersonDto> { _maleOwner1, _maleOwner1 };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 0);
        }

        [Test]
        public async Task Returns_EmptyModelIfThereAreNoOwners()
        {
            //Arrange
            _owners = new List<PersonDto> { };

            _mockPeopleService.Setup(s => s.GetPeople()).Returns(Task.FromResult<IEnumerable<PersonDto>>(_owners));

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act
            var catOwnersViewModel = await catsReadService.GetCatOwnersViewModel();

            //Assert
            Assert.AreEqual(catOwnersViewModel.CatsByGender.Count(), 0);
        }

        [Test]
        public void ThrowsException_WhenCannotCallPeopleApi()
        {
            //Arrange
            _owners = new List<PersonDto> { };

            _mockPeopleService.Setup(s => s.GetPeople()).Throws(new System.Exception());

            var catsReadService = new CatsReadService(_mockPeopleService.Object);

            //Act and Assert
            //TODO Ok, so this works, I may not fully understand it though... it basically just 
            //checks for any exception regardless of type - not sure if there is a better way to do
            //it or if this should be separated into a delegate somehow for the Act part, and the
            //delegate used in the Assert?
            Assert.CatchAsync(async () => await catsReadService.GetCatOwnersViewModel());
        }

    }
}