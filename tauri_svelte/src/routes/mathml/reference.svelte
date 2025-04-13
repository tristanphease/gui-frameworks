<script lang="ts">
    
    import reference from "./reference.json";

    let referenceId: number | null = $state(null);

    function showReference(index: number) {
        referenceId = index;
    }

    function hideReference() {
        referenceId = null;
    }

</script>

{#if referenceId != null}
{@const mathValue = reference.values[referenceId]}
    <div id="overlay-reference">
        <button
            style:position=absolute
            style:left=10px
            style:top=10px
            style:font-size=20px
            style:background-color=var(--bg-color)
            style:color=var(--text-color)
            onclick={hideReference}
        >
            Close
        </button>
        <h2>
            {mathValue.name}
        </h2>
        <span style:font-size=35px>
            <a href="https://developer.mozilla.org/en-US/docs/Web/MathML/Reference/Element/{mathValue.element}">
                &lt;{mathValue.element}&gt;
            </a>
        </span>
        <div style:padding=10px>
            {mathValue.description}
        </div>
        <span
            style:font-size=20px
        >
            Example(s):
        </span>
        <div>
            <table 
                style:text-align=center
            >
                <thead>
                    <tr>
                        <th>Text</th>
                        <th>Output</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{mathValue.example}</td>
                        <td>{@html mathValue.example}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
{/if}

<div id="reference-wrapper">
    {#each reference.values as mathValue, i}
        <button class="reference" onclick={() => showReference(i)}>
            <h3>{mathValue.name}</h3>
            <span>{mathValue.summary}</span>
        </button>
    {/each}
</div>


<style>
    #reference-wrapper {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 6px;
    }

    .reference {
        border: 3px solid white;
        border-radius: 10px;
        padding: 5px;
        text-align: center;
        background-color: var(--bg-color);
        color: var(--text-color);
    }

    #overlay-reference {
        position: absolute;
        text-align: center;
        width: 60%;
        background-color: var(--bg-color);
        border: 5px solid white;
        border-radius: 20px;
        display: flex;
        flex-direction: column;
        align-items: center;
    }

    th, td {
        border: 2px solid white;
        padding: 2px;
    }
</style>