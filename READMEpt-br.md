# Genshin Team Manager

Uma ferramenta para ver e comparar times, ou pares de times de Genshin Impact.

**Os dados necessários para usar essa ferramenta devem ser coletados manualmente, seja por cálculo manual ou usando ferramentas como [Genshin Optimizer](https://frzyc.github.io/genshin-optimizer/)**

(Glossário:)<br>
*DPM*: Dano por Minuto, a quantidade média de dano dado pelo time em um minuto <br>
*DPS*: Dano por Segundo, a quantidade média de dano dado pelo time em um segundo <br>
*Alvo Único*: Usado para considerar uma situação onde se luta com apenas um inimigo <br>
*AoE*: Área de Efeito, usado para considerar uma situação onde se luta com vários inimigos <br>

# Funções

## Página de Times

### Essa é a pagina para ver e comparar **Times Individuais**.

Será exibido uma tabela com todos os times registrados, seus membros, e o DPM (ou DPS) do respectivo time em Alvo Único e em AoE. Pode ser ordenado e buscado.

![uTKdG0QTPM](https://github.com/user-attachments/assets/f56be58c-c863-40e6-9fad-29825bf779c9)

Você pode clicar no nome de um time para mostrar seus detalhes, cada membro e seu respectivo dano para o total do time.

![image](https://github.com/user-attachments/assets/e28f5bd7-2633-4433-8105-efbc4fe5e8a6)

Se você checar a caixa de seleção *Comparar*, você pode selecionar outro time para compara-lo. Isso mostra em cor qual resultado é melhor ou pior, e em porcentagem, o quão melhor ou pior é.

![image](https://github.com/user-attachments/assets/68c09b27-1539-4f62-8709-8da1d150952a)


## Página de Pares de Times

### Esta é a página para visualizar e comparar *Pares de Times*, ou seja, dois times que não possuem membros em comum, de forma que possam ser sempre usadas no Abismo Espiral.

Exibe uma tabela com todas as combinações possíveis de pares de times registrados, junto com seus desempenhos em Alvo Único, Área de Efeito (AoE) e a média entre os dois. A tabela pode ser ordenada e pesquisada.

![SeG9uOGxcV](https://github.com/user-attachments/assets/2dc59868-8be5-46b3-af5a-795992aa3378)

Com a função de ordenação, você pode descobrir qual é o par de times teoricamente melhor para o Abismo (desconsiderando os inimigos). Se o Abismo tiver mais conteúdo de Alvo Único, escolha o par com o maior dano de Alvo Único. Se o Abismo tiver mais inimigos em ondas, escolha o par com o maior dano em AoE. Se o Abismo tiver ambos, escolha pela média.

Você também pode comparar dois pares.

![image](https://github.com/user-attachments/assets/f147a870-7f7e-4baa-b05b-612b21e5f830)

## Página de Edição de Times

### Bem autoexplicativo, nesta página você pode adicionar, editar, copiar e excluir times. Também possui uma tabela com todas as equipes para você selecionar.

![image](https://github.com/user-attachments/assets/3b448690-96a5-42b8-837a-a23715cc78ed)

Alterar o valor de Área de Efeito (AoE) aqui mudará apenas para essa equipe específica. Você pode usar isso caso, devido a constelação, buff da equipe ou estilo de jogo, a AoE seja alterada. Como, por exemplo, ao fazer um time de ataque imersivo com a Xianyun.

Quando você tenta adicionar ou alterar um personagem, você poderá escolher em uma grade que é filtrável e pesquisável.

![GMsGYuP2gc](https://github.com/user-attachments/assets/928b8554-242d-4061-a8fa-8fb0071b8475)

Ao adicionar um personagem pela primeira vez, ele usará seu multiplicador de AoE padrão e um valor bruto de dano (um valor independente).

Se, em vez disso, você clicar no botão rotulado como "R", será levado a mais detalhes sobre esse personagem.

![yYO1npHXvk](https://github.com/user-attachments/assets/9b768597-1f5b-409d-8c09-7163c3f011fc)

Dessa forma, você pode optar por usar **dano relativo**, ou seja, um multiplicador desse valor. Isso pode ser útil para evitar a alteração manual dos valores de dano em vários times. Caso você obtenha uma melhoria para esse personagem, alterar o valor base também aumentará o dano em todos os times que utilizam o mesmo perfil de dano.
Além disso, aqui você pode alterar a AoE global (que mudará a AoE para todas as equipes que usam o valor de AoE padrão desse personagem).

## Coisas Adicionais

Você pode escolher entre DPM ou DPS para ser usado em toda a ferramenta.
Você pode customizar a imagem exibida para os personagens.
Caso a ferramenta esteja desatualizada, você pode adicionar o personagem faltante.

---

Essa ferramenta foi feita para aprimorar meu conhecimento em C# e MAUI Blazor Hybrid, sendo este meu primeiro projeto utilizando o último.
Também foi usado [Mudblazor](https://mudblazor.com/) e [Drag and Drop Polyfill](https://gist.github.com/iain-fraser/01d35885477f4e29a5a638364040d4f2)

Crédito a [wanderer.moe](https://wanderer.moe/) pelos ícones padrões dos personagens.

### Versions
  * 0.8.3 <br>
        - Ajustes nas guias <br>
        - Corrigido aceitar valores inválidos de dano <br>
        - Corrigido endereços incorretos de certas imagens <br>
        - Novos personagens são adicionados alfabeticamente em seu elemento <br>
        - Leve re-esctrita e limpesa no código <br>
  * 0.8.2 <br>
        - Adicionado uma guia na página Principal (em inglês), para ajudar a navegar e conseguir os recursos necessários <br>
        - Leves mudanças à pagina de detalhes de personagem <br>
 * 0.8.1 <br>
        - Corrigido o travamento do aplicativo ao arrastar imagens <br>
        - Agora você pode arrastar os personagens por suas imagens em todas as páginas, exceto na página de Edição de Times <br>
        - Pré-lançamento desta versão disponível <br>
  * 0.8 <br>
        - Primeiro envio ao GitHub. Lançamento ainda não disponível <br>
  0.8 - Primeiro envio ao GitHub. Lançamento ainda não disponível.


