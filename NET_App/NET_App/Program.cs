var users = new Dictionary<int, (string, string, string)>() {
    { 1, ("Josip", "Josipovic", "08.11.1998")},
    { 2, ("Marko", "Markovic", "20.12.1996")},
    { 3, ("Frane", "Franic", "15.05.1955")},
    { 4, ("Ivan", "Ivic", "19.07.1963")},
    { 5, ("Petra", "Petric", "03.01.1977")},
};

var usersAccounts = new Dictionary<int, Dictionary<string, decimal>>() {
    {1, new Dictionary<string, decimal>{
            { "ziro", 1254.59m},
            { "tekuci", 256.00m},
            { "prepaid", 100.00m}
        }
    },
    {2, new Dictionary<string, decimal>{
            { "ziro", 557.00m},
            { "tekuci", 984.31m},
            { "prepaid", 20.00m}
        }
    },
    {3, new Dictionary<string, decimal>{
            { "ziro", 5012.95m},
            { "tekuci", -50.00m},
            { "prepaid", 0.00m}
        }
    },
    {4, new Dictionary<string, decimal>{
            { "ziro", 2120.15m},
            { "tekuci", 82.44m},
            { "prepaid", 52.03m}
        }
    },
    {5, new Dictionary<string, decimal>{
            { "ziro", -100.00m},
            { "tekuci", 32.98m},
            { "prepaid", 07.23m}
        }
    },
};

void createUser(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Taj korisnik vec postoji. Pokusajte ponovno.");
        Console.WriteLine();
    }

    Console.Write("Unesite ime novog korisnika: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime novog korisnika: ");
    var lastname = Console.ReadLine();
    Console.Write("Unesite datum rodenja novog korisnika: ");
    var birthDate = Console.ReadLine();

    foreach (var user in users)
    {
        if(user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            createUser(true);
        }
    }
    users.Add(users.Count + 1, (name, lastname, birthDate));
    Console.WriteLine("Korisnik uspjesno kreiran.");
    Console.WriteLine($"{name} - {lastname} - {birthDate}");
    startingMenu(false);
}

void reviewUsers()
{
    Console.Clear();
    var goBack = false;
    while (!goBack)
    {

        Console.WriteLine("0 - Pocetni meni");
        Console.WriteLine();

        foreach (var user in users)
        {
            Console.WriteLine($"{user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }

        Console.WriteLine();
        Console.Write("Odaberite radnju: ");
        var decision = Console.ReadLine();

        if (decision == "0")
        {
            goBack = true;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Pogresan unos. Pokusaj ponovno.");
            Console.WriteLine();
        }
    }
    
    startingMenu(false);

}

void usersMenu(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }
    

    Console.WriteLine("1 - Unos novog korisnika");
    Console.WriteLine("2 - Brisanje korisnika");
    Console.WriteLine("3 - Uredivanje korisnika");
    Console.WriteLine("4 - Pregled korisnika");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            createUser(false);
            break;
        case "2":
            // funkcija deleteUser
            break;
        case "3":
            // funkcija updateUser
            break;
        case "4":
            reviewUsers();
            break;
        case "0":
            startingMenu(false); // natrag na pocetni meni
            break;
        default:
            usersMenu(true);
            break;
    }
}

void startingMenu(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

    Console.WriteLine("1 - Korisnici");
    Console.WriteLine("2 - Racuni");
    Console.WriteLine("3 - Izlaz iz aplikacije");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            usersMenu(false);
            break;
        case "2":
            // funkcija account
            break;
        case "3":
            return;
        default:
            startingMenu(true);
            break;
    }
}

startingMenu(false);

