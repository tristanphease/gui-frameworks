
<script lang="ts">
    let y = 0;
    window.addEventListener("scroll", (e) => {
        console.log(e);
    });

    let height = 100000;
    window.onload = () => {
        const dummyElem = document.getElementById("dummy");
        console.log(dummyElem);
        if (dummyElem) {
            dummyElem.style.height = `${height}px`;
            console.log(`set height - ${dummyElem.style.height}`);
        }
    }
    
    

</script>

<svelte:window bind:scrollY={y} />
<main class="container">
    <div id="dummy" style="visibility: hidden;"></div>
    HELLO
</main>

