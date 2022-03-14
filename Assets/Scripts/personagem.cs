﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class personagem : MonoBehaviour
{
	//Função Mover
	public Rigidbody2D Rigidbody2DPersonagem;
	public float velocidade;

	//Função Rotacionar
	public SpriteRenderer SpriteRendererPersonagem;

	//Função Pular
	public int quantidadePulo = 0;
	public float meuTempoPulo = 0;
	public bool podePular = true;

	//Função VerificarCaindo
	public float auxCaindo = 0;
	public float tempoAux = 0;

	//Animator
	private Animator Animacao;

	//Função AtaqueBoomerang
	public GameObject Boomerang;
	public float tempoAnimacaoAtaque = 0;
	public bool podeAtacarBoomerang = true;
	public float tempoAtaque = 0;

	//Função Ataque
	public GameObject Ataque;

	//Função Dano
	public int vidaPersonagem = 5;
	public float meuTempoDano = 0;
	public bool podeTomarDano = true;

	//Função Vida HUD
	public int numCoracao = 5;

	public Image[] coracao;
	public Sprite coracaoSprite;
	public Sprite semCoracaoSprite;

	private int quantidadeMoedas = 0;
	private int quantidadeVidas = 3;
	private Text MoedaTexto;
	private Text VidasTexto;

	public GameObject GameObjectCheckPoint;
	public int atualCheckPoint = 0;
	public Vector3 coordenadasCheckPoint;

	public Color alpha;
	public bool morreu = false;

	private gerenciadorJogo GJ;

	//Start is called before the first frame update

	void Start()
	{

		GJ = GameObject.FindGameObjectWithTag("GameController").GetComponent<gerenciadorJogo>();

		coordenadasCheckPoint = new Vector3(-9.57f, -3.54f, 0);
		transform.position = coordenadasCheckPoint;
		Animacao = GetComponent<Animator>();
		MoedaTexto = GameObject.FindGameObjectWithTag("MoedaTexto").GetComponent<Text>();
		VidasTexto = GameObject.FindGameObjectWithTag("VidasTexto").GetComponent<Text>();
		VidasTexto.text = quantidadeVidas.ToString("00");
		
	}

	//Update is called once per frame

	void Update()
	{
		if (GJ.EstadoJogo() == true)
		{
			Mover();
			Rotacionar();
			Pular();
			AtaqueBoomerang();
			AtaqueCorpo();
			TemporizadorAnimacaoAtaque();
			PodeAtacar();
			Dano();
			Vida();
		}
	}

	void Mover()
	{
		velocidade = Input.GetAxis("Horizontal") * 5;
		Rigidbody2DPersonagem.velocity = new Vector2(velocidade, Rigidbody2DPersonagem.velocity.y);

		if (velocidade != 0)
		{
			Animacao.SetBool("Andando", true);
		}
		else
		{
			Animacao.SetBool("Andando", false);
		}
	}

	void Rotacionar()
	{
		if (velocidade > 0)
		{
			SpriteRendererPersonagem.flipX = false;
		}
		else if (velocidade < 0)
		{
			SpriteRendererPersonagem.flipX = true;
		}
	}

	void Pular()
	{
		if (Input.GetKeyDown(KeyCode.W) && podePular == true)
		{
			podePular = false;
			quantidadePulo++;
			if (quantidadePulo <= 2)
			{
				Rigidbody2DPersonagem.velocity = new Vector2(velocidade, 0);
				Rigidbody2DPersonagem.AddForce(transform.up * 500f);
			}
			if (quantidadePulo == 1)
			{
				Animacao.SetInteger("Pulando", 1);
			} else if (quantidadePulo >= 2)
			{
				Animacao.SetInteger("Pulando", 2);
			}
		}
		if (podePular == false)
		{
			TemporizadorPulo();
		}
	}

	void OnTriggerEnter2D(Collider2D trigger)
	{
		if (trigger.gameObject.tag == "Ground")
		{
			quantidadePulo = 0;
			meuTempoPulo = 0;
			Animacao.SetInteger("Pulando", 0);
		}
		if (trigger.gameObject.tag == "Moeda")
		{
			Destroy(trigger.gameObject);
			quantidadeMoedas++;
			MoedaTexto.text = quantidadeMoedas.ToString("000000000");
		}
		if (trigger.gameObject.tag == "CheckPoint")
		{
			if (atualCheckPoint < trigger.gameObject.GetComponent<checkPoint>().ponto)
			{
				atualCheckPoint = trigger.gameObject.GetComponent<checkPoint>().ponto;
				coordenadasCheckPoint = trigger.gameObject.transform.position;
				Destroy(trigger.gameObject);
			}
			else
			{
				Destroy(trigger.gameObject);
			}
		}
		if (trigger.gameObject.tag == "Agua")
		{
			if (morreu == false)
            {
				Morrer();
			}
		}
	}

	void TemporizadorPulo()
	{
		meuTempoPulo += Time.deltaTime;
		if (meuTempoPulo > 0.5f)
		{
			podePular = true;
			meuTempoPulo = 0;
		}
	}

	//Boomerang

	void AtaqueBoomerang()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0) && podeAtacarBoomerang == true)
		{
			Animacao.SetBool("Ataque", true);
			if (SpriteRendererPersonagem.flipX == false)
			{

				Vector3 pontoBoomerang = new Vector3(Rigidbody2DPersonagem.transform.position.x + 0.5f, Rigidbody2DPersonagem.transform.position.y, Rigidbody2DPersonagem.transform.position.z);
				GameObject BoomerangDisparo = Instantiate(Boomerang, pontoBoomerang, Quaternion.identity);
			}
			else
			{
				Vector3 pontoBoomerang = new Vector3(Rigidbody2DPersonagem.transform.position.x - 0.5f, Rigidbody2DPersonagem.transform.position.y, Rigidbody2DPersonagem.transform.position.z);
				GameObject BoomerangDisparo = Instantiate(Boomerang, pontoBoomerang, Quaternion.identity);
				BoomerangDisparo.GetComponent<SpriteRenderer>().flipX = true;
			}
			podeAtacarBoomerang = false;
		}
	}

	void AtaqueCorpo()
	{
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			Animacao.SetBool("Ataque", true);
			if (SpriteRendererPersonagem.flipX == false)
			{

				Vector3 pontoBoomerang = new Vector3(Rigidbody2DPersonagem.transform.position.x + 1f, Rigidbody2DPersonagem.transform.position.y, Rigidbody2DPersonagem.transform.position.z);
				GameObject DisparoAtaque = Instantiate(Ataque, pontoBoomerang, Quaternion.identity);
				Destroy(DisparoAtaque, 0.3f);
			}
			else
			{
				Vector3 pontoBoomerang = new Vector3(Rigidbody2DPersonagem.transform.position.x - 1f, Rigidbody2DPersonagem.transform.position.y, Rigidbody2DPersonagem.transform.position.z);
				GameObject DisparoAtaque = Instantiate(Ataque, pontoBoomerang, Quaternion.identity);
				DisparoAtaque.GetComponent<SpriteRenderer>().flipX = true;
				Destroy(DisparoAtaque, 0.3f);
			}
		}
	}

	void TemporizadorAnimacaoAtaque()
	{
		if (Animacao.GetBool("Ataque") == true)
		{
			tempoAnimacaoAtaque += Time.deltaTime;
			if (tempoAnimacaoAtaque >= 0.3f)
			{
				Animacao.SetBool("Ataque", false);
				tempoAnimacaoAtaque = 0;
			}
		}
	}

	void PodeAtacar()
	{
		if (podeAtacarBoomerang == false)
		{
			tempoAtaque += Time.deltaTime;
			if (tempoAtaque >= 1f)
			{
				podeAtacarBoomerang = true;
				tempoAtaque = 0;
			}
		}
	}

	//Dano

	void Dano()
	{
		if (podeTomarDano == false)
		{
			TemporizadorDano();
		}
	}

	void OnCollisionStay2D(Collision2D colisao)
	{
		if (colisao.gameObject.tag == "Inimigo")
		{
			if (podeTomarDano == true)
			{
				vidaPersonagem--;
				podeTomarDano = false;
				alpha = SpriteRendererPersonagem.material.color;
				alpha.a = 0.5f;
				SpriteRendererPersonagem.material.color = alpha;
				if (vidaPersonagem <= 0)
				{
					Morrer();
				}
			}
		}
		if (colisao.gameObject.tag == "Ovo")
		{
			if (podeTomarDano == true)
			{
				vidaPersonagem--;
				podeTomarDano = false;
				alpha = SpriteRendererPersonagem.material.color;
				alpha.a = 0.5f;
				SpriteRendererPersonagem.material.color = alpha;
				Destroy(colisao.gameObject);
				if (vidaPersonagem <= 0)
				{
					Morrer();
				}
			}
		}
	}

	void TemporizadorDano()
	{
		meuTempoDano += Time.deltaTime;
		if (meuTempoDano > 0.5f)
		{
			podeTomarDano = true;
			meuTempoDano = 0;
			alpha.a = 1f;
			SpriteRendererPersonagem.material.color = alpha;
		}
	}

	//Vida

	void Vida()
	{
		if (vidaPersonagem > numCoracao)
		{
			vidaPersonagem = numCoracao;
		}

		for (int i = 0; i < coracao.Length; i++)
		{
			if (i < vidaPersonagem)
			{
				coracao[i].sprite = coracaoSprite;
			}
			else
			{
				coracao[i].sprite = semCoracaoSprite;
			}

			if (i < numCoracao)
			{
				coracao[i].enabled = true;
			}
			else
			{
				coracao[i].enabled = false;
			}
		}
	}

	//Morte
	
	void Morrer()
	{
		quantidadeVidas--;
		VidasTexto.text = quantidadeVidas.ToString("00");
		if (quantidadeVidas > 0)
		{
			Inicializar();
		}
		else
		{
			GJ.PersonagemMorreu();
		}
	}

	void Inicializar()
	{
		transform.position = coordenadasCheckPoint;
		vidaPersonagem = 5;
		morreu = true;
	}
}