window.observeAndShrinkTables = () => {
    const resize = (table) => {
        let shrink = 1.0;
        const parent = table.parentElement;
        while (table.offsetWidth > parent.offsetWidth && shrink > 0.75) {
            shrink -= 0.05;
            table.style.fontSize = `${shrink * 100}%`;
            console.log(`Shrink applied to table: ${Math.round(shrink * 100)}%`);
        }
    };

    const tables = document.querySelectorAll('table');
    tables.forEach(table => {
        const observer = new ResizeObserver(() => resize(table));
        observer.observe(table);
        resize(table); // Initial check
    });
};
