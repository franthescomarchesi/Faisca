﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passaro : MonoBehaviour
{
	private SpriteRenderer SpriteRendererPassaro;
	private Vector3 PosicaoInicial;
	public float velocidade = 1f;
	public float distanciaInicial = -10.0f;
	public float distanciaFinal = 10.0f;
	private Animator Animacao;
	public float tempo = 0;
	public GameObject Ovo;
	public GameObject personagem;
	public int vidas = 2;
	private gerenciadorJogo GJ;
	float meuTempoDano;
	bool podeTomarDano = true;
	Color alpha;
	public AudioSource Hit;
	public AudioSource Passaro;

	void Start()
	{
		personagem = GameObject.FindGameObjectWithTag("Personagem");
		GJ = GameObject.FindGameObjectWithTag("GameController").GetComponent<gerenciadorJogo>();
		Hit = GameObject.FindGameObjectWithTag("Hit").GetComponent<AudioSource>();
		Animacao = GetComponent<Animator>();
		SpriteRendererPassaro = GetComponent<SpriteRenderer>();
		PosicaoInicial = transform.position;
	}

	void Update()
	{
		if (GJ.EstadoJogo() == true)
		{
			iniciarScriptsInimigo();
		}
	}

	void Andar()
	{
		transform.position = new Vector3(transform.position.x + velocidade * Time.deltaTime, transform.position.y, transform.position.z);
		if (transform.position.x > (PosicaoInicial.x + distanciaFinal))
		{
			velocidade = -Mathf.Abs(velocidade);
			SpriteRendererPassaro.flipX = true;
		}
		else if (transform.position.x < (PosicaoInicial.x + distanciaInicial))
		{
			velocidade = Mathf.Abs(velocidade);
			SpriteRendererPassaro.flipX = false;
		}
	}

	void TempoOvo()
	{
		tempo += Time.deltaTime;
		if (tempo >= 5.0f)
		{
			AtaqueOvo();
			tempo = 0;
		}
	}

	void AtaqueOvo()
	{
		Vector3 pontoOvo = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
		GameObject OvoDisparo = Instantiate(Ovo, pontoOvo, Quaternion.identity);
	}

	private void OnTriggerStay2D(Collider2D colisao)
	{
		if (colisao.gameObject.tag == "DestroyBoomerang")
		{
			if (podeTomarDano)
			{
				Hit.Play();
				podeTomarDano = false;
				Destroy(colisao.gameObject);
				alpha = GetComponent<SpriteRenderer>().material.color;
				alpha.a = 0.5f;
				GetComponent<SpriteRenderer>().material.color = alpha;
				vidas--;
				if (vidas <= 0)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
	void iniciarScriptsInimigo()
	{
		if (Vector2.Distance(transform.position, personagem.transform.position) <= 30f)
        {
			if (!Passaro.isPlaying)
			{
				Passaro.Play();
			}
			Andar();
			TempoOvo();
			Dano();
		}
	}

	void Dano()
	{
		if (!podeTomarDano)
		{
			TemporizadorDano();
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
			GetComponent<SpriteRenderer>().material.color = alpha;
		}
	}
}
