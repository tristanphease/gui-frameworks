
<script lang="ts">
    import { scrollBehaviour } from "$lib/scroll.svelte.js";
    
    let boxColour = $state("#ff0000");
    
    function transformFunction(value: number) {
        const r = Math.floor(value * 255);
        const g = Math.floor((1 - value) * 255);
        const b = 0;
        const a = 1 - 2 * Math.abs(value - 0.5);

        boxColour = colourString(r, g, b, a);
    }

    function colourString(r: number, g: number, b: number, a = 1): string {
        return `rgb(${r}, ${g}, ${b}, ${a})`;
    }

</script>

<main class="container">

    <div id="background">
        <div id="box" style:background-color={boxColour} use:scrollBehaviour={[ transformFunction, 30000 ]}>
            
        </div>
    </div>
    
</main>

<style>
    #box {
        width: 100vw;
        height: 100vh;
    }

    #background {
        background-color: #181818;
    }

</style>

