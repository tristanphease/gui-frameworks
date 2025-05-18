<script lang="ts">
    import symbols from "./symbols.json";
</script>

<div id="symbols">
    <h2>Mathematical Symbols</h2>
    <p>
        Most symbols are just implemented as the symbol themselves.
        Obviously some symbols are just on the keyboard, e.g. <br />
        plus: + <br />
        subtract: - <br />
        equals: = <br />
        The digits 0-9 themselves obviously. <br />
        However, other symbols require other ways to represent them, which is where entity codes are used. <br />
        <a href="https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references#List_of_character_entity_references_in_HTML">
            https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references#List_of_character_entity_references_in_HTML
        </a>
    </p>

    <table id="symbols-wrapper">
        <thead>
            <tr>
                <th>Name</th>
                <th>Entity code</th>
                <th>Entity</th>
            </tr>
        </thead>
        <tbody>
            {#each symbols.values as symbol}
                <tr>
                    <td>{symbol.name}</td>
                    <td>{symbol.symbol}</td>
                    <td>{@html symbol.symbol}</td>
                </tr>
            {/each}
        </tbody>
        
    </table>
    
</div>

<style>
    th, td {
        border: 1px solid white;
        padding: 2px;
    }
</style>