# Clasoft
<b>Clasoft</b> é um sistema online de matrículas e manutenção de grades curriculares. 
Foi desenvolvido por <a href="https://www.mariomadureira.com.br" target="_blank">Mario Madureira</a> para fins de estudo 
e prática de desenvolvimento.

## Ficha Técnica
<ul>
<li>Arquitetura MVC + DAO</li>
<li>.NET Framework v4.7.2</li>
<li>Entity Framework v6.0.0.0</li>
<li>Bootstrap v4.3.1</li>
<li>Font Awesome Free v5.9.0</li>
<li>JQuery v3.4.1</li>
<li>JQuery Easing v1.4.1</li>
<li>Start Bootstrap - SB Admin 2 v4.0.6</li>
</ul>

## Regras de Negócio
Uma faculdade pretende informatizar seu sistema de matrículas.
A secretaria da faculdade gera o currículo para cada semestre e mantém as informações sobre as disciplinas, professores e alunos.

Cada curso tem um nome, um determinado número de créditos e é constituído por diversas disciplinas.

Os alunos podem se matricular a 4 disciplinas como 1ª opção e a mais 2 outras alternativas.

Há períodos para efetuar matrículas, durante os quais um aluno pode acessar o sistema para se matricular
em disciplinas e/ou para cancelar matrículas feitas anteriormente.

Uma disciplina só fica ativa, isto é, só irá funcionar no semestre seguinte se, no final do período de
matrículas tiver, pelo menos, 3 alunos inscritos (matriculados). Caso contrário, a disciplina será cancelada.

O número máximo de alunos inscritos a uma disciplina é de 10 e quando este número é atingido, as inscrições
(matrículas) a essa disciplina são encerradas.

Após um aluno se inscrever para um semestre, o sistema de cobranças é notificado pelo sistema de matrículas,
de modo que o aluno possa ser cobrado pelas disciplinas daquele semestre.

Os professores podem acessar o sistema para saber quais são os alunos que estão matriculados em cada disciplina.
Todos os usuários do sistema têm senhas que são utilizadas para validação do respectivo login.

## Solução

As regras de negócio estão contempladas em diversas operações. Essas funções estão agrupadas em módulos afim de prover
uma experiência intuitiva ao usuário.

### Módulo Sistema
Reúne as operações relacionadas a criação de usuário, alteração de senha e análise de ocorrência de erro a partir 
de registros de operação do sistema.

#### Usuário
Criação, manutenção de usuários e atribuição de perfil de acesso.

#### Log
Análise de ocorrência de erro a partir de registros de operação do sistema

### Módulo Cadastro
Reúne as operações relacionadas a criação e edição de informações de curso, disciplina e semestre. 

#### Curso
Criação e manutenção de informações de curso.

#### Disciplina
Criação e manutenção de informações de disciplina. É necessário que haja, pelo menos, 1 curso cadastrado no sistema.

#### Semestre
Criação e manutenção de informações de semestre. O sistema exige que o usuário realize o cadastro manualmente para iniciar 
a manutenção da nova grade.

### Módulo Grade
Reúne as operações relacionadas a criação e manutenção de grades de cursos, liberação para matrícula e fechamento da mesma.

#### Manutenção
Criação e manutenção de grades de curso. É necessário que haja informações de curso, disciplina, semestre e professor cadastradas 
anteriormente. A funcionalidade também permite a cópia entre grades para facilitar a mudança de semestre e também exige a definição
do preço de matrícula.

Importante destacar a regra que define que, caso haja uma mudança no estado atual da grade (Ex: Liberada para Cadastrada), 
as matrículas serão excluídas.

#### Liberação
Revisão de todas as grades editadas e a liberação das mesmas para os alunos se matricularem.

#### Fechamento
Panorama de matrículas por grade e o encerramento do período de matrículas. A função verifica a quantidade mínima de 3 matrículas
para a grade ser encerrada. Caso contrário, automaticamente é cancelada.

### Módulo Aluno
Reúne as operações relacionadas a criação e manutenção de informações de aluno e matrícula de disciplinas.

#### Novo
Insere um novo aluno na base do sistema. É importante que o e-mail seja único para a criação do usuário para o aluno.

#### Manutenção
Edição de informações de aluno previamente cadastrados.

#### Inscrições
Panorama de matrículas realizadas por semestre e o formulário para realização da matrícula. Cada aluno pode cancelar inscrições
realizadas no semestre atual e incluir novas, sendo que cada um só pode incluir 4 disciplinas como primeira opção e 2 como segunda
no total. O sistema verifica se as disciplinas atingiram a quantidade máxima de inscritos para remover a grade do formulário 
de matrículas.

### Módulo Professor
Reúne as operações relacionadas a criação e manutenção de informações de professor e a lista de alunos matriculados em cada disciplina
do mesmo.

#### Novo
Insere um novo professor na base do sistema. É importante que o e-mail seja único para a criação do usuário para o professor.

#### Manutenção
Edição de informações de professor previamente cadastrados.

#### Lista de Chamada
Relação de alunos matriculados por disciplina e semestre, após a grade estar encerrada.

## Aviso

Este projeto foi desenvolvido exclusivamente para fins de prática de estudo.<br>
Fica <b>proibida</b> a utilização do código para outra finalidade a não ser o citado anteriormente.
