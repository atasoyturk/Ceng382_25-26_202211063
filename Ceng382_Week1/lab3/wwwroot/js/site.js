// changes card text
const button_text = document.getElementById('change_text');
const text = document.querySelector('.card__text'); 

button_text.addEventListener("click", function(){
    text.textContent = "Yazı değişti";
});


// changes table order
const button_table = document.getElementById('change_table');

button_table.addEventListener("click", function(){
    const tbody = document.querySelector('.dataTable tbody');
    const rows = Array.from(tbody.rows); 

    rows.sort(function(a,b){
        const grade_a = a.cells[4].innerText;
        const grade_b = b.cells[4].innerText;

        return grade_a > grade_b ? 1 : -1;
    });

    rows.forEach(function(row){
        tbody.appendChild(row);
    });
});

//changes image
const button_png = document.getElementById('change_image');

button_png.addEventListener("click", function(){
    const image = document.querySelector('.profile_image');
    image.src = "anormal.png";
})