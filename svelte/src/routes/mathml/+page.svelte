
<script lang="ts">
    import Examples from "./examples.svelte";
    import Reference from "./reference.svelte";

    import Symbols from "./symbols.svelte";
</script>


<main class="container">
    <div id="main-wrapper">

        <div id="intro-box">
            <h2>
                The basics of MathML
            </h2>
            <p>
                MathML is a way to write mathematical notation on the web in a consistent way with other elements.

                e.g. you can write things like this:
            </p>
            <p>
                <math>
                    <msup>
                        <mn>a</mn>
                        <mn>2</mn>
                    </msup>
                    <mo>+</mo>
                    <msup>
                        <mn>b</mn>
                        <mn>2</mn>
                    </msup>
                    <mo>=</mo>
                    <msup>
                        <mn>c</mn>
                        <mn>2</mn>
                    </msup>
                </math>
            </p>
            <p>
                Without resorting to complex things like svg rendering. It's semantically consistent and it works with css styling and javascript.
            </p>
            <div>
                Useful links:
                <ul>
                    <li><a href="https://w3c.github.io/mathml-core/">https://w3c.github.io/mathml-core/</a></li>
                    <li><a href="https://developer.mozilla.org/en-US/docs/Web/MathML">https://developer.mozilla.org/en-US/docs/Web/MathML</a></li>
                </ul>
            </div>
        </div>

        <Reference />

        <Symbols />

        <Examples />

    </div>
    
</main>

<style>

    * {
        font-family: 'Courier New', Courier, monospace;
        --bg-color: rgb(10, 10, 10);
        --text-color: white;
    }

    #intro-box {
        background-color: var(--bg-color);
        text-align: center;
        
    }

    #main-wrapper {
        width: 60%;
        padding: 10px;
    }

    .container {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 20px;
        margin: 20px;
    }

</style>

