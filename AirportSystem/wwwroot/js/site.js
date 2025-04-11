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
});
