# Changelog

## 2025-07-21
- **Refaktoryzacja i Poprawki Błędów:**
    - Zrefaktoryzowano zarządzanie pamięcią podręczną dla obiektów `WorkOrder` w `ApiServiceClient`, aby zapewnić spójność danych. Wprowadzono scentralizowane metody do aktualizacji, unieważniania i odświeżania pamięci podręcznej (`UpdateWorkOrderInCache`, `InvalidateWorkOrdersCache`, `RefreshWorkOrderInCacheAsync`).
    - Wszystkie metody manipulacji `WorkOrder` (`Update`, `Save`, `Close`) korzystają teraz z nowej, scentralizowanej logiki pamięci podręcznej.
    - Poprawiono logikę aktualizacji interfejsu użytkownika w `SchedulerPage.razor`. Lokalna kolekcja spotkań (`_allAppointments`) jest teraz poprawnie modyfikowana po zmianie lub zamknięciu spotkania, co zapewnia, że interfejs użytkownika odzwierciedla aktualny stan.
    - Ulepszono `AppointmentEditor.razor`, aby obsługiwał zamykanie zleceń pracy i poprawnie propagował aktualizacje interfejsu użytkownika.
    - Dodano model `WorkOrderInfo.cs` i zaktualizowano `WoController`, aby obsługiwał pobieranie szczegółowych informacji o zleceniach pracy.

## 2025-07-09
- **Funkcjonalność i Interfejs Użytkownika:**
    - Wdrożono pełny cykl życia dla Zleceń Pracy (Work Orders), umożliwiając ich tworzenie, aktualizację i zamykanie przez API.
    - Dodano solidną walidację dla wymaganych pól (takich jak Opis, Poziom) w edytorze Zleceń Pracy, z wizualnym podświetleniem brakujących danych.
    - Zamykanie Zlecenia Pracy wymaga teraz co najmniej jednej powiązanej czynności (activity).
    - Wyskakujące powiadomienia wyświetlają teraz konkretne komunikaty o błędach z API, zapewniając użytkownikowi jaśniejszą informację zwrotną.
- **Techniczne i Refaktoryzacja:**
    - Wprowadzono sprawdzanie sesji po stronie serwera przy ładowaniu aplikacji. Użytkownicy są teraz automatycznie wylogowywani, jeśli ich sesja na serwerze jest nieprawidłowa, co zapobiega błędom API.
    - Zrefaktoryzowano metody `ApiServiceClient` (`Save`, `Update`, `Close`), aby zwracały obiekt `SingleResponse<T>`, dostarczając szczegółowych informacji o błędach.
    - Dodano dedykowane punkty końcowe API i modele żądań (`WorkOrderCreateRequest`, `UpdateWorkOrderRequest`, `CloseWorkOrderRequest`) dla wszystkich operacji na Zleceniach Pracy.
    - Przełączono się z używania nazw w postaci ciągów znaków na identyfikatory (ID) dla encji takich jak Kategorie i Poziomy, co poprawia integralność danych.

## 2025-07-08
- **Ulepszenie zarządzania zleceniami pracy**: Zaimplementowano walidację i mapowanie `WorkOrder` na `WorkOrderCreateRequest` po stronie klienta w `ApiServiceClient.SaveWorkOrderAsync`. Punkt końcowy `WoController.Create` teraz bezpośrednio akceptuje `WorkOrderCreateRequest`. `ApiServiceClient.UpdateWorkOrderAsync` wykorzystuje teraz `SaveWorkOrderAsync` dla nowych zleceń pracy, zapewniając spójne zapisywanie do API. Dodano pola `ReasonID`, `CategoryID`, `DepartmentID` i `AssignedPersonID` do `WorkOrderCreateRequest`.
- **Synchronizacja tokenów między klientem a serwerem**: Zaimplementowano solidny mechanizm, w którym serwer Blazor pobiera tokeny zewnętrznego API ze swojej pamięci podręcznej, używając `PersonID` dostarczonego przez klienta w niestandardowym nagłówku HTTP `X-Person-ID`. Eliminuje to potrzebę bezpośredniego wysyłania tokena zewnętrznego API z klienta do serwera Blazor.
- **Ulepszenie inicjalizacji po stronie klienta**: Upewniono się, że dane `UserState` są w pełni załadowane z `localStorage` przed wykonaniem jakichkolwiek wywołań API po stronie klienta. Osiągnięto to poprzez oczekiwanie na `UserState.InitializationTask` w `AuthHeaderHandler` i odpowiednich stronach Razor (`SchedulerPage`, `OrdersPage`, `Settings`).
- **Zarządzanie tokenami po stronie serwera**: `IdentityController` teraz buforuje `IdentityData` (w tym token zewnętrznego API) po pomyślnym zalogowaniu użytkownika. `ServerAuthTokenService` został zrefaktoryzowany w celu pobierania tych tokenów z pamięci podręcznej serwera (`IMemoryCache`) na podstawie `PersonID` wyodrębnionego z nagłówka `X-Person-ID`.
- **Obsługa błędów API**: Dodano bloki `try-catch` do wszystkich metod żądań HTTP w `ApiServiceClient`, aby elegancko obsługiwać błędy sieciowe i API, logując je do konsoli i zwracając puste/domyślne wartości.
- **Udoskonalenie i czyszczenie zależności**: Usunięto przestarzałe konfiguracje uwierzytelniania JWT Bearer i zakodowane na stałe dane logowania z pliku `Program.cs` po stronie serwera, usprawniając konfigurację uwierzytelniania i wstrzykiwania zależności.
- **Dokumentacja API**: Dodano `API-info.md` w celu zapewnienia jasnej dokumentacji punktów końcowych API i ich typów autoryzacji.

## 2025-07-07
- **Interfejs użytkownika i funkcjonalność:**
    - Ulepszone logowanie z rozwijaną listą wyszukiwania nazwy użytkownika i podstawową walidacją danych wejściowych.
    - Ulepszony edytor zleceń pracy z dynamicznym wyborem kategorii i poziomów oraz ulepszonym wyświetlaniem urządzeń/kategorii.
    - Automatyczne buforowanie nowych wpisów słownikowych (kategorie, poziomy, działy).
    - Udoskonalono logikę filtrowania zleceń pracy na stronie harmonogramu.
    - Ulepszona obsługa błędów klienta API.
- **Zmiany w kodzie:**
    - Usunięto parametr `ListDicts` z `AppointmentEditor.razor`.
    - Zaktualizowano `WorkOrderComponent.razor` do używania `ListCategories` i `ListLevels` dla TelerikComboBox, usunięto logikę inicjalizacji `ListDicts` i dodano logikę aktualizacji `selectedDevice`.
    - Zastąpiono bezpośrednie manipulacje `ListDicts` wywołaniami `apiService.AddNewWODict` dla kategorii, działów i poziomów.
    - Zastąpiono pole wprowadzania nazwy użytkownika komponentem `TelerikComboBox` w `Login.razor`.
    - Dodano wskaźnik ładowania i usunięto parametr `ListDicts` ze `SchedulerPage.razor`.
    - Zmodyfikowano logikę filtrowania zleceń pracy w `SchedulerPage.razor`.
    - Dodano `_dictCache` i nowe metody związane ze słownikami (`GetWODictionaries`, `GetWODictionariesCached`, `GetWOCategories`, `GetWOStates`, `GetWOLevels`, `GetWOReasons`, `GetWODepartments`, `AddNewWODict`) do `ApiServiceClient.cs`.
    - Ulepszono obsługę błędów w `PostSingleAsync` w `ApiServiceClient.cs`.
    - Dodano konstruktor bez parametrów do `UserState.cs`.
    - Zmieniono punkt końcowy logowania na `api/v1/identity/loginpass` i metodę na GET w `IdentityController.cs`.
    - Zarejestrowano `UserState` jako usługę Scoped w `Program.cs`.

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
    - Dodano właściwość `Expires` do modelu `IdentityData`.
    - Zrefaktoryzowano `AuthHeaderHandler` w celu użycia `IdentityData` i `ApiResponse<IdentityData>` do zarządzania tokenami.
    - Zmodyfikowano `IdentityController.cs` w celu zwracania pełnych danych `IdentityData` z zewnętrznego API.
    - Ulepszono `UserState` w celu utrwalania `IdentityData` w `localStorage` i ładowania jej przy uruchomieniu.
    - Zmodyfikowano `Login.razor` w celu jawnego zapisywania `IdentityData` w `UserState` (a tym samym w `localStorage`) po pomyślnym zalogowaniu.

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
    - Zaktualizowano `ApiServiceClient` do pobierania obiekt��w `Dict`.
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
