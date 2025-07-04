# Changelog

## 2025-07-04
- **Interfejs użytkownika i funkcjonalność:**
    - Zaimplementowano autoryzację użytkownika za pośrednictwem strony `Login.razor`.
    - Zintegrowano `UserState` do zarządzania danymi logowania i tokenem bieżącego użytkownika.
- **Zmiany w kodzie:**
    - Dodano właściwość `Password` do `UserState.cs`.
    - Zrefaktoryzowano `AuthHeaderHandler.cs` w celu pobierania danych uwierzytelniających użytkownika z `UserState` do odświeżania tokena.
    - Zmodyfikowano `BlazorTool.Client/Program.cs` w celu prawidłowego wstrzykiwania `UserState` do `AuthHeaderHandler`.
    - Dodano metodę `PostSingleAsync` do `ApiServiceClient.cs` do obsługi odpowiedzi API zawierających pojedynczy obiekt.
    - Zaktualizowano `Login.razor` w celu użycia `ApiServiceClient.PostSingleAsync` do uwierzytelniania i aktualizacji `UserState` po pomyślnym zalogowaniu.
    - Uzupełniono model `RightMatrix` w celu prawidłowej deserializacji uprawnień użytkownika.

## 2025-07-04
- **Zmiany w kodzie:**
    - Dodano właściwość `Expires` do modelu `IdentityData`.
    - Zrefaktoryzowano `AuthHeaderHandler` w celu użycia `IdentityData` i `ApiResponse<IdentityData>` do zarządzania tokenami.
    - Zmodyfikowano `IdentityController.cs` w celu zwracania pełnych danych `IdentityData` z zewnętrznego API.

## 2025-07-03
- **Interfejs użytkownika i funkcjonalność:**
    - Zaimplementowano tworzenie nowych spotkań w Harmonogramie.
    - Dodano pole kombi do wyboru urządzenia podczas tworzenia nowego spotkania.
    - Zaimplementowano walidację wymaganych pól w Edytorze Spotkań.
    - Dodano wyskakujące powiadomienie informujące użytkowników o niewypełnionych wymaganych polach.
    - Zmieniono nazwę pola "Title" na "AssetNo" w Edytorze Spotkań i podświetlono je, jeśli jest puste.
    - Ulepszono stronę `ChangelogPage` w celu renderowania Markdown dla lepszej czytelności.
- **Zmiany w kodzie:**
    - Dodano logikę do `AppointmentService` do obsługi tworzenia nowych spotkań.
    - Zaimplementowano zapisywanie nowych spotkań za pośrednictwem `ApiServiceClient`.
    - Dodano rozwijaną listę do wyboru `Device` w `WorkOrderComponent` dla nowych spotkań.
    - Dodano logikę walidacji w `AppointmentEditor.razor`.
    - Zaimplementowano wyskakujące okienko Telerik do powiadomień w `AppointmentEditor.razor`.
    - Zaktualizowano style w `AppointmentEditor.razor.css`.
    - Dodano pakiet `Markdig` do renderowania Markdown na stronie `ChangelogPage`.

## 2025-07-02
- **Interfejs użytkownika i funkcjonalność:**
    - Dodano filtr statusu na stronie `OrdersPage`.
    - Poprawiono logikę filtrowania na stronie `OrdersPage`.
    - Dodano opisowe etykiety dla filtrów.
    - Dodano sumaryczną pracochłonność do nagłówka listy działań.
    - Komponent `WorkOrderComponent` teraz rozszerza się, aby pokazać pełną listę działań bez przewijania.
    - Wyrównano rozmiary kolumn między nagłówkiem w `ActivityList` a elementami w `ActivityDisplay`, aby uzyskać spójny wygląd.
    - Użytkownicy mogą teraz klikać na statystykę "Akcje" w widoku zlecenia pracy, aby zobaczyć szczegółową listę działań.
    - Lista działań jest wyświetlana w kompaktowym, czytelnym formacie tabeli.
- **Zmiany w kodzie:**
    - Dodano obliczanie `totalWorkload` w `ActivityList.razor`.
    - Użyto warunkowej klasy CSS w `WorkOrderComponent.razor` do dostosowania `max-height`, gdy działania są widoczne.
    - Poprawiono deserializację JSON dla modelu `Activity`, dodając `[JsonConstructor]` do domyślnego konstruktora.
    - Utworzono model `Activity.cs`.
    - Dodano `ActivityController` do pobierania danych o działaniach z zewnętrznego API.
    - Zaimplementowano `GetActivityByWO` w `ApiServiceClient` w celu pobierania działań.
    - Opracowano komponenty Blazor `ActivityList` i `ActivityDisplay` do wyświetlania działań.
    - Zmodyfikowano `WorkOrderComponent` w celu wyświetlania listy działań po interakcji użytkownika.
    - Porządek na ChangelogPage

## 2025-07-01
- **Interfejs użytkownika i funkcjonalność:**
    - Na stronie `SchedulerPage` dodano filtr `Device` po `AssetNo` za filtrem `Department`.
    - Umożliwiono edycję `WorkOrderComponent`, dodając Telerik ComboBox dla pola `WOCategory`, `WOLevel`.
    - W komponencie `WorkOrderComponent` wyróżniono puste pola `Department` i `StartDate`.
    - Na stronie `SchedulerPage` zmieniono logikę wyświetlania zajętych/niezajętych zleceń.
    - Na stronie `SchedulerPage` dostosowano tekst i kolor elementu harmonogramu w zależności od jego stanu.
- **Zmiany w kodzie:**
    - Dodano nowy punkt końcowy `api/v1/wo/getdict` do `WoController` w celu pobierania kategorii zleceń pracy.
    - Zaktualizowano `ApiServiceClient` do pobierania obiektów `Dict`.
    - Dodano model `Dict.cs`.
- **Poprawki błędów:**
    - Naprawiono problem w `WorkOrderComponent`, gdzie `Department` i `assignedPersons` nie wyświetlały się poprawnie.

## 2025-06-30
- **Interfejs użytkownika i funkcjonalność:**
    - Umożliwiono edycję komponentu `WorkOrderComponent`, dodając Telerik components dla pola `assignedPersons`, `Department` i `Description`.
    - Na stronie `OrderPage` dodano kolumnę 'Device'.
    - Na stronie `SchedulerPage` dodano funkcjonalność `OnClick` dla zleceń (aby przeglądać WorkOrder na liście zleceń) oraz zaimplementowano filtry nagłówków dla zleceń.
- **Zmiany w kodzie:**
    - Utwożono i dodano plik `telerik_manual.md` z instrukcjami użycia komponentów Telerik.

## 2025-06-27
- Dodano stronę Dziennika zmian i zaktualizowano mechanizm logowania.
- Wprowadzono pliki `ChangelotPage.razor` i `CHANGELOG.md` do obsługi informacji o wydaniu.
- Zastąpiono `Debug.Print` przez `Console.WriteLine` w celu ujednolicenia logowania po stronie klienta i serwera.
- Drobne poprawki w interfejsie użytkownika (UI) w plikach `MainLayout.razor` i `OrdersPage.razor`.
- Skonfigurowano kopiowanie pliku `CHANGELOG.md` do katalogu `wwwroot` w celu zapewnienia dostępu po stronie klienta.
- Zaimplementowano wyświetlanie zawartości pliku `CHANGELOG.md` na stronie `ChangelogPage.razor`.
- Skonfigurowano domyślny `HttpClient` w `Program.cs` z `BaseAddress` dla dostępu do plików statycznych.
- Poprawiono atrybut `Link` w pliku `BlazorTool.Client.csproj` dla `CHANGELOG.md`, aby zapewnić jego prawidłowe umieszczenie w `wwwroot`.
- Wycofano zmianę `app.UseBlazorFrameworkFiles()` w `BlazorTool/Program.cs`, ponieważ powodowała problemy z ładowaniem aplikacji.
- Ponownie dodano rejestrację `HttpClient` z `BaseAddress` i sprawdzaniem `serverBaseUrl` w `BlazorTool.Client/Program.cs`.
- **SchedulerPage.razor:**
    - Dodano filtr kategorii urządzeń do siatki nieprzypisanych zleceń.
    - Zaimplementowano pole wyboru (checkbox) do pokazywania/ukrywania zrealizowanych zleceń w siatce nieprzypisanych zleceń.
    - Dodano zdarzenie `OnChange` dla pola wielokrotnego wyboru (multiselect) z przypisanymi osobami, aby wyzwalać natychmiastową aktualizację widoku.
    - Dostosowano układ kontrolek filtrowania i widoku zleceń.

## 2025-06-26
- Ulepszono interfejs użytkownika (UI) Schedulera i zaktualizowano tryb renderowania Blazor.
- Skonfigurowano klienta HttpClient po stronie klienckiej, aby obsługiwał dynamiczny bazowy adres URL serwera.
- Zaktualizowano plik .gitignore.
- Zaimplementowano `PersonController` i zrefaktoryzowano klienta API do obsługi zewnętrznych wywołań.

## 2025-06-25
- W trakcie implementacji: dodawanie filtrów do Schedulera.
- Komponent `WorkOrderComponent`: dodano przewijanie, bardziej kompaktowy widok danych, dynamiczne kolory statusu. Strona `OrdersPage`: wyrównano elementy filtra w jednej linii.

## 2025-06-24
- Dodano filtry na stronie Zleceń, w tym pole wielokrotnego wyboru (multiSelect).
- Naprawiono problemy z autoryzacją (AUTH), dodano różne klienty HTTP.
- Refaktoryzacja pliku `Program.cs`: wstrzykiwanie zależności (DI), dodano `AuthHeaderHandler` do pobierania tokenu, usunięto pobieranie tokenu dla `ApiServiceClient`, dodano `HostAddress` w `appSettings` dla serwera.
- Wyrównano przyciski w edytorze Schedulera.

## 2025-06-23
- Zaimplementowano metody `Save/Delete` w `AppointmentService` oraz `Save/DeleteAppointment` w `ApiServiceClient`..
- Dodano widok terminów w Schedulerze oraz funkcjonalność przesuwania/aktualizowania elementów.
- Zaktualizowano klasę `SchedulerAppointment`, aby dziedziczyła po `WorkOrder`.
- Ustawiono komponent `WorkOrderComponent` jako tylko do odczytu.

## 2025-06-20
- Skonfigurowano układ 3-kolumnowy na stronie Zleceń.
- Włączono otwieranie zleceń po kliknięciu.
- Zaimplementowano sprawdzanie połączenia z API serwera oraz zapis i odczyt ustawień.
- Dodano kontroler `SettingsController`.
- Dodano stronę Ustawień (`Settings.razor`) z opcją `TestApiAddress` i metodą `CheckApiAddress` w `ApiServiceClient`.
- Dodano stronę Zamówienia (`OrderPage`).