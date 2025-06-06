$(document).ready(function () {
    const FADE_DURATION = 300;   
    const ROTATE_DURATION = 800; 
    let isFlipping = false;    
    $(".js-flip-flight").on("click", function (e) {
        e.preventDefault();

        if (isFlipping) return;
        isFlipping = true;

        const $arrowWrapper = $(this);
        const flightId = $arrowWrapper.data("flight-id");
        const $icon = $arrowWrapper.find("i");
        const $card = $arrowWrapper.closest(".card");
        const $depContainer = $card.find(".dep-container");
        const $arrContainer = $card.find(".arr-container");

        $icon.addClass("rotate-animation");
        $depContainer.addClass("fade-out");
        $arrContainer.addClass("fade-out");

        $.ajax({
            type: "POST",
            url: "/Flights/FlipAirports",
            data: { id: flightId },
            success: function (response) {
                if (!response.success) {
                    alert(response.error || "Flip failed.");
                    removeAnimations();
                    return;
                }

                setTimeout(function () {
                    $depContainer.find(".airport-name").text(response.departureAirportName);
                    $depContainer.find(".airport-time").text(response.departureTime);
                    $arrContainer.find(".airport-name").text(response.arrivalAirportName);
                    $arrContainer.find(".airport-time").text(response.arrivalTime);

                    $depContainer.removeClass("fade-out").addClass("fade-in");
                    $arrContainer.removeClass("fade-out").addClass("fade-in");

                    setTimeout(function () {
                        $depContainer.removeClass("fade-in");
                        $arrContainer.removeClass("fade-in");
                    }, FADE_DURATION);
                }, FADE_DURATION);
            },
            error: function () {
                alert("Error flipping flight. Server unreachable?");
                removeAnimations();
            },
            complete: function () {
                setTimeout(function () {
                    $icon.removeClass("rotate-animation");
                    isFlipping = false;
                }, ROTATE_DURATION);
            }
        });

        function removeAnimations() {
            $icon.removeClass("rotate-animation");
            $depContainer.removeClass("fade-out fade-in");
            $arrContainer.removeClass("fade-out fade-in");
            isFlipping = false;
        }
    });
    // --- LIVE SPOTLIGHT FILTER LOGIC ---
    const searchInput = $('#spotlightSearchInput');
    const noResultsMessage = $('#no-results');

    if (searchInput.length > 0) { // Only run if the search input exists on the page
        searchInput.on('input', function () {
            const searchTerm = $(this).val().toLowerCase().trim();
            let visibleCards = 0;

            // Target the containers we added the class to
            $('.card-container').each(function () {
                const $cardContainer = $(this);
                // Get all text content from the card inside the container
                const cardText = $cardContainer.find('.card, .airport-board').text().toLowerCase();

                if (cardText.includes(searchTerm)) {
                    $cardContainer.removeClass('dimmed-out');
                    visibleCards++;
                } else {
                    $cardContainer.addClass('dimmed-out');
                }
            });

            // Show or hide the "no results" message
            if (visibleCards === 0 && searchTerm !== "") {
                noResultsMessage.show();
            } else {
                noResultsMessage.hide();
            }
        });
    }
    const rouletteBtn = $('#startRouletteBtn');

    if (rouletteBtn.length) { 
        let isRouletteRunning = false;

        rouletteBtn.on('click', function () {
            if (isRouletteRunning) return; 

            const cards = $('.card-container');
            if (cards.length < 2) return; 

            isRouletteRunning = true;
            rouletteBtn.prop('disabled', true).text('Завъртане...');

            cards.removeClass('roulette-winner roulette-spin-highlight');
            $('#card-list-container').addClass('roulette-active');


            let spinDuration = 3000; 
            let spinInterval = 100;  
            let spinTimer;

            const spin = () => {
                cards.removeClass('roulette-spin-highlight');
                $(cards[Math.floor(Math.random() * cards.length)]).addClass('roulette-spin-highlight');
            };

            spinTimer = setInterval(spin, spinInterval);


            setTimeout(() => {
                clearInterval(spinTimer); 

                const winnerCard = $(cards[Math.floor(Math.random() * cards.length)]);

                cards.removeClass('roulette-spin-highlight');
                winnerCard.addClass('roulette-winner');

                winnerCard[0].scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });

                $('#card-list-container').removeClass('roulette-active');
                rouletteBtn.prop('disabled', false).html('<i class="fa-solid fa-dice-d20 me-2"></i> Завърти отново?');
                isRouletteRunning = false;

            }, spinDuration);
        });
    }
});
