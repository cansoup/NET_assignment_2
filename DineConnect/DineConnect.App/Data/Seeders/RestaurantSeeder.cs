using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DineConnect.App.Models;
using DineConnect.App.Data.Seeders;

public class RestaurantSeeder : ISeeder
{
    public async Task SeedAsync(DineConnectContext db)
    {
        if (!await db.Restaurants.AnyAsync())
        {
            db.Restaurants.AddRange(
            // Sydney CBD & Circular Quay
            new Restaurant{ Id = 20000001, Name = "Quay", Address = "Upper Level, Overseas Passenger Terminal, The Rocks NSW 2000", Lat = -33.8587, Lng = 151.2107, Phone = "(02) 9251 5600" }, 
            new Restaurant { Id = 20000002, Name = "Bennelong", Address = "Bennelong Point, Sydney NSW 2000", Lat = -33.8572, Lng = 151.2152, Phone = "(02) 9240 8000" }, 
            new Restaurant { Id = 20000003, Name = "Aria Restaurant Sydney", Address = "1 Macquarie St, Sydney NSW 2000", Lat = -33.8588, Lng = 151.2132, Phone = "(02) 9240 2255" }, 
            new Restaurant { Id = 20000004, Name = "Mr. Wong", Address = "3 Bridge Ln, Sydney NSW 2000", Lat = -33.8638, Lng = 151.2081, Phone = "(02) 9114 7317" }, 
            new Restaurant { Id = 20000005, Name = "Rockpool Bar & Grill", Address = "66 Hunter St, Sydney NSW 2000", Lat = -33.8658, Lng = 151.2091, Phone = "(02) 8099 7077" }, 
            new Restaurant { Id = 20000006, Name = "Cafe Sydney", Address = "Customs House, 31 Alfred St, Sydney NSW 2000", Lat = -33.8611, Lng = 151.2109, Phone = "(02) 9251 8683" }, 
            new Restaurant { Id = 20000007, Name = "Restaurant Hubert", Address = "15 Bligh St, Sydney NSW 2000", Lat = -33.8660, Lng = 151.2104, Phone = "(02) 9232 0881" },
            // Surry Hills
            new Restaurant { Id = 20000008, Name = "Firedoor", Address = "23-33 Mary St, Surry Hills NSW 2010", Lat = -33.8824, Lng = 151.2118, Phone = "(02) 8204 0800" },
            new Restaurant { Id = 20000009, Name = "Chin Chin Sydney", Address = "69 Commonwealth St, Surry Hills NSW 2010", Lat = -33.8791, Lng = 151.2106, Phone = "(02) 9281 3322" }, 
            new Restaurant { Id = 20000010, Name = "Poly", Address = "74-76 Commonwealth St, Surry Hills NSW 2010", Lat = -33.8793, Lng = 151.2106, Phone = "(02) 8860 0808" }, 
            new Restaurant { Id = 20000011, Name = "Arthur", Address = "544 Bourke St, Surry Hills NSW 2010", Lat = -33.8860, Lng = 151.2163, Phone = "(02) 9331 4265" }, 
            new Restaurant { Id = 20000012, Name = "The Dolphin Hotel", Address = "412 Crown St, Surry Hills NSW 2010", Lat = -33.8858, Lng = 151.2144, Phone = "(02) 9331 4800" }, 
            // Bondi
            new Restaurant { Id = 20000013, Name = "Icebergs Dining Room and Bar", Address = "1 Notts Ave, Bondi Beach NSW 2026", Lat = -33.8931, Lng = 151.2755, Phone = "(02) 9130 3120" }, 
            new Restaurant { Id = 20000014, Name = "Sean's", Address = "270 Campbell Parade, North Bondi NSW 2026", Lat = -33.8887, Lng = 151.2828, Phone = "(02) 9365 4924" }, 
            new Restaurant { Id = 20000015, Name = "Totti's", Address = "283 Bondi Rd, Bondi NSW 2026", Lat = -33.8943, Lng = 151.2662, Phone = "(02) 9114 7371" }, 
            // The Rocks
            new Restaurant { Id = 20000016, Name = "Sake Restaurant & Bar", Address = "12 Argyle St, The Rocks NSW 2000", Lat = -33.8596, Lng = 151.2089, Phone = "(02) 9259 5656" }, 
            new Restaurant { Id = 20000017, Name = "The Glenmore Hotel", Address = "96 Cumberland St, The Rocks NSW 2000", Lat = -33.8587, Lng = 151.2069, Phone = "(02) 9247 4794" }, 
            // Newtown
            new Restaurant { Id = 20000018, Name = "Bella Brutta", Address = "135 King St, Newtown NSW 2042", Lat = -33.8953, Lng = 151.1818, Phone = "(02) 9922 5941" }, 
            new Restaurant { Id = 20000019, Name = "Continental Deli Bar Bistro", Address = "210 Australia St, Newtown NSW 2042", Lat = -33.8980, Lng = 151.1783, Phone = "(02) 8624 3131" }, 
            new Restaurant { Id = 20000020, Name = "Mary's", Address = "6 Mary St, Newtown NSW 2042", Lat = -33.8969, Lng = 151.1834, Phone = "N/A" }, 
            // Potts Point & Darlinghurst
            new Restaurant { Id = 20000021, Name = "Ms.G's", Address = "155 Victoria St, Potts Point NSW 2011", Lat = -33.8722, Lng = 151.2248, Phone = "(02) 9114 7352" }, 
            new Restaurant { Id = 20000022, Name = "The Apollo", Address = "44 Macleay St, Potts Point NSW 2011", Lat = -33.8698, Lng = 151.2255, Phone = "(02) 8354 0888" }, 
            new Restaurant { Id = 20000023, Name = "Fratelli Paradiso", Address = "12-16 Challis Ave, Potts Point NSW 2011", Lat = -33.8711, Lng = 151.2263, Phone = "(02) 9357 1744" }, 
            // Barangaroo & Darling Harbour
            new Restaurant { Id = 20000024, Name = "Cirrus Dining", Address = "23 Barangaroo Ave, Barangaroo NSW 2000", Lat = -33.8647, Lng = 151.2020, Phone = "(02) 9220 0111" }, 
            new Restaurant { Id = 20000025, Name = "Bea Restaurant", Address = "35 Barangaroo Ave, Barangaroo NSW 2000", Lat = -33.8640, Lng = 151.2017, Phone = "(02) 8298 9910" }, 
            // Paddington
            new Restaurant { Id = 20000026, Name = "Saint Peter", Address = "362 Oxford St, Paddington NSW 2021", Lat = -33.8837, Lng = 151.2335, Phone = "(02) 8937 2530" }, 
            new Restaurant { Id = 20000027, Name = "Fred's", Address = "380 Oxford St, Paddington NSW 2021", Lat = -33.8840, Lng = 151.2343, Phone = "(02) 9114 7331" }, 
            new Restaurant { Id = 20000028, Name = "10 William St", Address = "10 William St, Paddington NSW 2021", Lat = -33.8824, Lng = 151.2269, Phone = "(02) 9360 3310" }, 
            // Chatswood
            new Restaurant { Id = 20000029, Name = "Mama Mulan", Address = "Level 1/260 Victoria Ave, Chatswood NSW 2067", Lat = -33.7963, Lng = 151.1837, Phone = "(02) 8080 7888" }, 
            new Restaurant { Id = 20000030, Name = "Din Tai Fung", Address = "Westfield Chatswood, Anderson St, Chatswood NSW 2067", Lat = -33.7968, Lng = 151.1802, Phone = "(02) 9415 3155" }, 
            // Manly
            new Restaurant { Id = 20000031, Name = "Hugos Manly", Address = "Manly Wharf, East Esplanade, Manly NSW 2095", Lat = -33.8016, Lng = 151.2854, Phone = "(02) 9977 1000" }, 
            new Restaurant { Id = 20000032, Name = "Sake Restaurant & Bar Manly", Address = "Manly Wharf, Belgrave St, Manly NSW 2095", Lat = -33.8021, Lng = 151.2858, Phone = "(02) 8076 7226" }, 
            // Parramatta
            new Restaurant { Id = 20000033, Name = "Temasek", Address = "71 George St, Parramatta NSW 2150", Lat = -33.8155, Lng = 151.0069, Phone = "(02) 9633 9926" }, 
            new Restaurant { Id = 20000034, Name = "BAYVISTA", Address = "330 Church St, Parramatta NSW 2150", Lat = -33.8122, Lng = 151.0028, Phone = "(02) 9067 2100" }, 
            // Etc
            new Restaurant { Id = 20000035, Name = "The Grounds of Alexandria", Address = "7a/2 Huntley St, Alexandria NSW 2015", Lat = -33.9103, Lng = 151.1923, Phone = "(02) 9699 2225" }, 
            new Restaurant { Id = 20000036, Name = "Ester Restaurant", Address = "46-52 Meagher St, Chippendale NSW 2008", Lat = -33.8872, Lng = 151.1989, Phone = "(02) 8068 8279" }, 
            new Restaurant { Id = 20000037, Name = "Sixpenny", Address = "83 Percival Rd, Stanmore NSW 2048", Lat = -33.8943, Lng = 151.1643, Phone = "(02) 9560 6660" }, 
            new Restaurant { Id = 20000038, Name = "Lankan Filling Station", Address = "58 Riley St, East Sydney NSW 2011", Lat = -33.8767, Lng = 151.2157, Phone = "N/A" }, 
            new Restaurant { Id = 20000039, Name = "Alberto's Lounge", Address = "17-19 Alberta St, Sydney NSW 2000", Lat = -33.8782, Lng = 151.2091, Phone = "(02) 9114 7331" }, 
            new Restaurant { Id = 20000040, Name = "Pellegrino 2000", Address = "80 Campbell St, Surry Hills NSW 2010", Lat = -33.8804, Lng = 151.2096, Phone = "N/A" }, 
            new Restaurant { Id = 20000041, Name = "Mimi's", Address = "130a Beach St, Coogee NSW 2034", Lat = -33.9210, Lng = 151.2583, Phone = "(02) 9114 7324" }, 
            new Restaurant { Id = 20000042, Name = "Bathers' Pavilion", Address = "4 The Esplanade, Mosman NSW 2088", Lat = -33.8290, Lng = 151.2587, Phone = "(02) 9969 5050" }, 
            new Restaurant { Id = 20000043, Name = "Ormeggio at The Spit", Address = "D'Albora Marinas, Spit Rd, Mosman NSW 2088", Lat = -33.8183, Lng = 151.2467, Phone = "(02) 9969 4088" }, 
            new Restaurant { Id = 20000044, Name = "Bert's Bar & Brasserie", Address = "2 Kalinya St, Newport NSW 2106", Lat = -33.6625, Lng = 151.3168, Phone = "(02) 9114 7350" }, 
            new Restaurant { Id = 20000045, Name = "Cottage Point Inn", Address = "2 Anderson Pl, Cottage Point NSW 2084", Lat = -33.6429, Lng = 151.2023, Phone = "(02) 9456 1011" }, 
            new Restaurant { Id = 20000046, Name = "Catalina Rose Bay", Address = "Lyne Park, Rose Bay NSW 2029", Lat = -33.8710, Lng = 151.2642, Phone = "(02) 9371 0555" }, 
            new Restaurant { Id = 20000047, Name = "Tetsuya's", Address = "529 Kent St, Sydney NSW 2000", Lat = -33.8759, Lng = 151.2046, Phone = "(02) 9267 2900" }, 
            new Restaurant { Id = 20000048, Name = "Automata", Address = "5 Kensington St, Chippendale NSW 2008", Lat = -33.8845, Lng = 151.2008, Phone = "(02) 8277 8555" }, 
            new Restaurant { Id = 20000049, Name = "LuMi Dining", Address = "56 Pirrama Rd, Pyrmont NSW 2009", Lat = -33.8687, Lng = 151.1963, Phone = "(02) 9571 1999" }, 
            new Restaurant { Id = 20000050, Name = "The Gantry Restaurant", Address = "11 Hickson Rd, Dawes Point NSW 2000", Lat = -33.8569, Lng = 151.2047, Phone = "(02) 8298 9910" }, 
            new Restaurant { Id = 20000051, Name = "Porteño", Address = "50 Holt St, Surry Hills NSW 2010", Lat = -33.8846, Lng = 151.2104, Phone = "(02) 8399 1440" }, 
            new Restaurant { Id = 20000052, Name = "Golden Century", Address = "393-399 Sussex St, Sydney NSW 2000", Lat = -33.8783, Lng = 151.2044, Phone = "(02) 9212 3901" }, 
            new Restaurant { Id = 20000053, Name = "Chat Thai", Address = "20 Campbell St, Haymarket NSW 2000", Lat = -33.8797, Lng = 151.2057, Phone = "(02) 9211 1808" }, 
            new Restaurant { Id = 20000054, Name = "Mamak", Address = "15 Goulburn St, Haymarket NSW 2000", Lat = -33.8793, Lng = 151.2051, Phone = "(02) 9211 1668" }, 
            new Restaurant { Id = 20000055, Name = "Marigold", Address = "683-689 George St, Sydney NSW 2000", Lat = -33.8792, Lng = 151.2062, Phone = "(02) 9281 3388" }, 
            new Restaurant { Id = 20000056, Name = "Spice I Am", Address = "90 Wentworth Ave, Surry Hills NSW 2010", Lat = -33.8797, Lng = 151.2114, Phone = "(02) 9280 0928" }, 
            new Restaurant { Id = 20000057, Name = "Bodega", Address = "216 Commonwealth St, Surry Hills NSW 2010", Lat = -33.8824, Lng = 151.2118, Phone = "(02) 9212 7766" }, 
            new Restaurant { Id = 20000058, Name = "Longrain", Address = "85 Commonwealth St, Surry Hills NSW 2010", Lat = -33.8796, Lng = 151.2108, Phone = "(02) 9280 2888" }, 
            new Restaurant { Id = 20000059, Name = "Billy Kwong", Address = "1/28 Macleay St, Potts Point NSW 2011", Lat = -33.8694, Lng = 151.2252, Phone = "N/A" }, 
            new Restaurant { Id = 20000060, Name = "Cho Cho San", Address = "73 Macleay St, Potts Point NSW 2011", Lat = -33.8704, Lng = 151.2259, Phone = "(02) 9331 6601" }, 
            new Restaurant { Id = 20000061, Name = "Momofuku Seiobo", Address = "The Star, 80 Pyrmont St, Pyrmont NSW 2009", Lat = -33.8679, Lng = 151.1947, Phone = "N/A" }, 
            new Restaurant { Id = 20000062, Name = "Flying Fish", Address = "Jones Bay Wharf, 19-21 Pirrama Rd, Pyrmont NSW 2009", Lat = -33.8683, Lng = 151.1953, Phone = "(02) 9518 6677" }, 
            new Restaurant { Id = 20000063, Name = "Sokyo", Address = "The Star, 80 Pyrmont St, Pyrmont NSW 2009", Lat = -33.8678, Lng = 151.1950, Phone = "(02) 9657 9161" }, 
            new Restaurant { Id = 20000064, Name = "BLACK Bar & Grill", Address = "The Star, 80 Pyrmont St, Pyrmont NSW 2009", Lat = -33.8677, Lng = 151.1951, Phone = "(02) 9657 9109" }, 
            new Restaurant { Id = 20000065, Name = "The Bridge Room", Address = "44 Bridge St, Sydney NSW 2000", Lat = -33.8637, Lng = 151.2102, Phone = "(02) 9247 7000" }, 
            new Restaurant { Id = 20000066, Name = "Felix", Address = "2 Ash St, Sydney NSW 2000", Lat = -33.8690, Lng = 151.2070, Phone = "(02) 9114 7303" }, 
            new Restaurant { Id = 20000067, Name = "Bambini Trust Restaurant & Cafe", Address = "185 Elizabeth St, Sydney NSW 2000", Lat = -33.8732, Lng = 151.2093, Phone = "(02) 9283 7098" }, 
            new Restaurant { Id = 20000068, Name = "Azuma Japanese Restaurant", Address = "Chifley Plaza, 2 Chifley Square, Sydney NSW 2000", Lat = -33.8665, Lng = 151.2115, Phone = "(02) 9222 9960" }, 
            new Restaurant { Id = 20000069, Name = "The Baxter Inn", Address = "152-156 Clarence St, Sydney NSW 2000", Lat = -33.8704, Lng = 151.2052, Phone = "N/A" }, 
            new Restaurant { Id = 20000070, Name = "Frankie's Pizza", Address = "50 Hunter St, Sydney NSW 2000", Lat = -33.8662, Lng = 151.2085, Phone = "N/A" }, 
            new Restaurant { Id = 20000071, Name = "Palmer & Co.", Address = "Abercrombie Ln, Sydney NSW 2000", Lat = -33.8643, Lng = 151.2079, Phone = "(02) 9114 7315" }, 
            new Restaurant { Id = 20000072, Name = "Shady Pines Saloon", Address = "4/256 Crown St, Darlinghurst NSW 2010", Lat = -33.8787, Lng = 151.2152, Phone = "N/A" }, 
            new Restaurant { Id = 20000073, Name = "Bar Reggio", Address = "135 Crown St, Darlinghurst NSW 2010", Lat = -33.8753, Lng = 151.2144, Phone = "(02) 9332 1129" }, 
            new Restaurant { Id = 20000074, Name = "Lucio Pizzeria", Address = "248 Palmer St, Darlinghurst NSW 2010", Lat = -33.8784, Lng = 151.2166, Phone = "(02) 9380 5996" }, 
            new Restaurant { Id = 20000075, Name = "bills Darlinghurst", Address = "433 Liverpool St, Darlinghurst NSW 2010", Lat = -33.8797, Lng = 151.2195, Phone = "(02) 9360 9631" }, 
            new Restaurant { Id = 20000076, Name = "Una's", Address = "340 Victoria St, Darlinghurst NSW 2010", Lat = -33.8775, Lng = 151.2223, Phone = "(02) 9360 9265" }, 
            new Restaurant { Id = 20000077, Name = "Gelato Messina Darlinghurst", Address = "241 Victoria St, Darlinghurst NSW 2010", Lat = -33.8761, Lng = 151.2215, Phone = "(02) 9331 1588" }, 
            new Restaurant { Id = 20000078, Name = "Macleay Street Bistro", Address = "73A Macleay St, Potts Point NSW 2011", Lat = -33.8704, Lng = 151.2259, Phone = "(02) 9358 4891" }, 
            new Restaurant { Id = 20000079, Name = "Yellow", Address = "57 Macleay St, Potts Point NSW 2011", Lat = -33.8701, Lng = 151.2257, Phone = "(02) 9332 2344" }, 
            new Restaurant { Id = 20000080, Name = "Dear Sainte Éloise", Address = "5/29 Orwell St, Potts Point NSW 2011", Lat = -33.8732, Lng = 151.2241, Phone = "(02) 9326 9740" }, 
            new Restaurant { Id = 20000081, Name = "The Butler Potts Point", Address = "123 Victoria St, Potts Point NSW 2011", Lat = -33.8719, Lng = 151.2245, Phone = "(02) 8354 0742" }, 
            new Restaurant { Id = 20000082, Name = "The Fish Shop", Address = "22 Challis Ave, Potts Point NSW 2011", Lat = -33.8714, Lng = 151.2265, Phone = "(02) 9326 9000" }, 
            new Restaurant { Id = 20000083, Name = "Et Al.", Address = "7/24-30 Springfield Ave, Potts Point NSW 2011", Lat = -33.8735, Lng = 151.2228, Phone = "(02) 9356 8393" }, 
            new Restaurant { Id = 20000084, Name = "L'altro", Address = "231 Victoria St, Darlinghurst NSW 2010", Lat = -33.8760, Lng = 151.2214, Phone = "N/A" }, 
            new Restaurant { Id = 20000085, Name = "Sonora", Address = "37 Bayswater Rd, Potts Point NSW 2011", Lat = -33.8750, Lng = 151.2230, Phone = "(02) 9160 8887" }, 
            new Restaurant { Id = 20000086, Name = "Raja", Address = "1/106-110 Bayswater Rd, Rushcutters Bay NSW 2011", Lat = -33.8756, Lng = 151.2254, Phone = "(02) 8084 8393" }, 
            new Restaurant { Id = 20000087, Name = "Marta", Address = "30 McLachlan Ave, Rushcutters Bay NSW 2011", Lat = -33.8773, Lng = 151.2253, Phone = "(02) 9361 4749" }, 
            new Restaurant { Id = 20000088, Name = "Sydney Fish Market", Address = "Corner Pyrmont Bridge Rd &, Bank St, Pyrmont NSW 2009", Lat = -33.8732, Lng = 151.1925, Phone = "(02) 9004 1100" }, 
            new Restaurant { Id = 20000089, Name = "Boathouse on Blackwattle Bay", Address = "123 Glebe Point Rd, Glebe NSW 2037", Lat = -33.8735, Lng = 151.1852, Phone = "(02) 9518 9011" }, 
            new Restaurant { Id = 20000090, Name = "Glebe Point Diner", Address = "407 Glebe Point Rd, Glebe NSW 2037", Lat = -33.8797, Lng = 151.1820, Phone = "(02) 9660 2646" }, 
            new Restaurant { Id = 20000091, Name = "Lotus Dumpling Bar", Address = "3/16 Hickson Rd, Dawes Point NSW 2000", Lat = -33.8569, Lng = 151.2059, Phone = "(02) 9251 8328" }, 
            new Restaurant { Id = 20000092, Name = "The Lord Nelson Brewery Hotel", Address = "19 Kent St, The Rocks NSW 2000", Lat = -33.8601, Lng = 151.2037, Phone = "(02) 9251 4044" }, 
            new Restaurant { Id = 20000093, Name = "Fortune of War", Address = "137 George St, The Rocks NSW 2000", Lat = -33.8598, Lng = 151.2082, Phone = "(02) 9247 2714" }, 
            new Restaurant { Id = 20000094, Name = "Pancakes On The Rocks", Address = "4 Hickson Rd, The Rocks NSW 2000", Lat = -33.8591, Lng = 151.2064, Phone = "(02) 9247 6371" }, 
            new Restaurant { Id = 20000095, Name = "Guylian Belgian Chocolate Café", Address = "1-27 Circular Quay W, The Rocks NSW 2000", Lat = -33.8602, Lng = 151.2093, Phone = "(02) 8274 7900" }, 
            new Restaurant { Id = 20000096, Name = "Aqua Dining", Address = "Paul St, Milsons Point NSW 2061", Lat = -33.8510, Lng = 151.2119, Phone = "(02) 9964 9998" }, 
            new Restaurant { Id = 20000097, Name = "Ripples Milsons Point", Address = "Olympic Dr, Milsons Point NSW 2061", Lat = -33.8515, Lng = 151.2117, Phone = "(02) 9929 7722" }, 
            new Restaurant { Id = 20000098, Name = "The Deck Sydney", Address = "1 Olympic Dr, Milsons Point NSW 2061", Lat = -33.8517, Lng = 151.2115, Phone = "(02) 9033 7670" }, new Restaurant { Id = 20000099, Name = "Lavendra", Address = "1/73-75 Walker St, North Sydney NSW 2060", Lat = -33.8398, Lng = 151.2081, Phone = "(02) 9955 0377" }, 
            new Restaurant { Id = 20000100, Name = "Woodlands Eatery", Address = "3/24-30 Springfield Ave, Potts Point NSW 2011", Lat = -33.8735, Lng = 151.2228, Phone = "N/A" }
            );
            await db.SaveChangesAsync();
        }
    }
}
